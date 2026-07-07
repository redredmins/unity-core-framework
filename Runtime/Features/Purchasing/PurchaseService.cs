using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// 인앱 결제. Unity IAP 5(StoreController) 사용.
// 게임 고유 요소(SKU/튜토리얼·게스트 게이팅/문자열 테이블/토스트/애널리틱스)는 제거하고
// 초기화 시 상품 목록 + IPurchaseHandler로 역전. 에디터에서는 결제 없이 즉시 지급.
namespace RedMinS
{
    public class ProductEntry
    {
        public string id;
        public ProductType type;

        public ProductEntry(string id, ProductType type)
        {
            this.id = id;
            this.type = type;
        }
    }

    public interface IPurchaseHandler
    {
        // 아이템 지급. true 반환 시 결제를 확정(ConfirmPurchase)한다.
        bool GrantProduct(string productId, Order order);
        void OnPurchaseFailed(string productId, string reason);
        // Muug의 WriteLogInDB/WritePurchaseInDB 대체 훅
        void OnPurchaseLog(string message);
    }

    public static class PurchaseService
    {
        static IReadOnlyList<ProductEntry> products = null;
        static IPurchaseHandler handler = null;

#if UNITY_EDITOR
        public static void Initialize(IReadOnlyList<ProductEntry> products, IPurchaseHandler handler)
        {
            PurchaseService.products = products;
            PurchaseService.handler = handler;
        }

        public static void RestoreTransactions(Action<bool, int> onComplete)
        {
            if (onComplete != null) onComplete(false, 0);
        }

        public static void BuyProduct(string id, Action onSuccess)
        {
            if (handler != null) handler.GrantProduct(id, null);
            if (onSuccess != null) onSuccess();
        }

#elif UNITY_ANDROID || UNITY_IPHONE
        static StoreController store = null;
        static bool isConnecting = false;
        static bool isProductsReady = false;

        static string buyingSku = null;
        static Action buyingCallback = null;

        static int restoredCount = 0;

        // 스토어 연결 + 상품 조회 (인트로에서 1회 호출, 실패 시 구매 시점에 재시도)
        public static async void Initialize(IReadOnlyList<ProductEntry> products, IPurchaseHandler handler)
        {
            PurchaseService.products = products;
            PurchaseService.handler = handler;

            if (store != null || isConnecting) return;
            isConnecting = true;

            try
            {
                var controller = UnityIAPServices.StoreController();
                controller.OnPurchasePending += OnPurchasePending;
                controller.OnPurchaseFailed += OnPurchaseFailed;
                controller.OnProductsFetched += (fetched) => { isProductsReady = true; };
                controller.OnProductsFetchFailed += (fail) =>
                {
                    if (handler != null) handler.OnPurchaseLog("InApp_fetch_fail >> " + fail.ToString());
                };

                await controller.Connect();
                store = controller;

                var defs = new List<ProductDefinition>();
                if (products != null)
                {
                    foreach (var p in products)
                    {
                        defs.Add(new ProductDefinition(p.id, p.type));
                    }
                }
                store.FetchProducts(defs);
            }
            catch (Exception e)
            {
                if (handler != null) handler.OnPurchaseLog("InApp_init_fail >> " + e.Message);
            }
            finally
            {
                isConnecting = false;
            }
        }

        // 복원 버튼용. 완료 콜백(성공여부, 이번에 복원 도착한 개수)
        public static void RestoreTransactions(Action<bool, int> onComplete)
        {
            if (store == null)
            {
                if (onComplete != null) onComplete(false, 0);
                return;
            }
            restoredCount = 0;
            store.RestoreTransactions((success, error) =>
            {
                if (handler != null) handler.OnPurchaseLog("InApp_restore >> " + success + " / " + error);
                if (onComplete != null) onComplete(success, restoredCount);
            });
        }

        public static void BuyProduct(string id, Action onSuccess)
        {
            if (store == null || isProductsReady == false)
            {
                Initialize(products, handler); // 재시도 준비
                if (handler != null) handler.OnPurchaseFailed(id, "store_not_ready");
                return;
            }

            buyingSku = id;
            buyingCallback = onSuccess;
            store.PurchaseProduct(id);
        }

        static void OnPurchasePending(PendingOrder order)
        {
            var items = order.CartOrdered.Items();
            string productId = (items.Count > 0) ? items[0].Product.definition.id : "";

            bool isActive = (productId == buyingSku && buyingCallback != null);
            Action callback = null;
            if (isActive)
            {
                callback = buyingCallback;
                buyingSku = null;
                buyingCallback = null;
            }
            else
            {
                // 이전 세션의 미확정 주문 등: 지급 컨텍스트가 없어 게임 훅에 위임
                restoredCount++;
            }

            bool granted = (handler == null) || handler.GrantProduct(productId, order);

            if (handler != null)
            {
                handler.OnPurchaseLog((isActive ? "InApp_success >> " : "InApp_pending_restored >> ") + productId);
            }

            if (granted)
            {
                if (isActive && callback != null) callback();
                store.ConfirmPurchase(order);
            }
        }

        static void OnPurchaseFailed(FailedOrder order)
        {
            string productId = buyingSku;
            buyingSku = null;
            buyingCallback = null;

            if (handler != null)
            {
                handler.OnPurchaseFailed(productId, order.FailureReason.ToString());
                handler.OnPurchaseLog("InApp_fail_result >> " + order.FailureReason + " / " + order.Details);
            }
        }
#endif
    }
}

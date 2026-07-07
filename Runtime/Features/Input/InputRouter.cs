using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RedMinS
{
    // 입력 커맨드 종류 (커맨드 패턴): 원시 입력을 프레임 단위 커맨드로 변환한다
    public enum InputCommandType
    {
        PointerDown,   // 포인터(마우스/터치) 눌림 시작
        PointerHold,   // 눌림 유지 (매 프레임)
        PointerUp,     // 뗌
        Back,          // 안드로이드 뒤로가기 / ESC
        MoveUp,        // 방향 입력 (키보드 화살표 등)
        MoveDown,
        MoveLeft,
        MoveRight,
    }

    public struct InputCommand
    {
        public InputCommandType type;
        public Vector2 pointerPosition; // 포인터 계열 커맨드에서 유효 (스크린 좌표)

        public InputCommand(InputCommandType type, Vector2 pointerPosition = default)
        {
            this.type = type;
            this.pointerPosition = pointerPosition;
        }
    }

    // 커맨드를 소비하면 true 반환 → 낮은 우선순위 핸들러로 전달 중단
    public delegate bool InputCommandHandler(in InputCommand command);

    // 공용 입력 라우터 (Core.app.input)
    // - 신 InputSystem(Pointer/Keyboard)에서 원시 입력을 읽어 InputCommand로 변환,
    //   우선순위 핸들러 체인에 디스패치한다
    // - Simulate()로 커맨드를 주입할 수 있다 (튜토리얼 자동 재생, 테스트, 리플레이)
    // - 기존 폴링 코드 이식용 프레임 상태 프로퍼티 제공 (커맨드 스트림에서 파생 -
    //   레거시 Input.GetMouseButton* 와 동일한 프레임 의미론)
    [DefaultExecutionOrder(-1000)] // 소비자 Update보다 먼저 폴링 (레거시 Input의 프레임 의미론 보존)
    public class InputRouter : MonoBehaviour
    {
        // --- 폴링 뷰: 레거시 대응은 다음과 같다
        // Input.GetMouseButtonDown(0) -> PointerDownThisFrame
        // Input.GetMouseButton(0)     -> PointerHeld
        // Input.GetMouseButtonUp(0)   -> PointerUpThisFrame
        // Input.mousePosition         -> PointerPosition
        // Input.GetKeyDown(Escape)    -> BackPressedThisFrame
        public bool PointerDownThisFrame { get; private set; }
        public bool PointerUpThisFrame { get; private set; }
        public bool PointerHeld { get; private set; }
        public Vector2 PointerPosition { get; private set; }
        public bool BackPressedThisFrame { get; private set; }

        struct Entry
        {
            public InputCommandHandler handler;
            public int priority;
        }

        readonly List<Entry> handlers = new List<Entry>();
        readonly List<InputCommand> simulated = new List<InputCommand>();

        // 우선순위가 높을수록 먼저 받는다 (예: 팝업 > 게임플레이)
        public void AddHandler(InputCommandHandler handler, int priority = 0)
        {
            handlers.Add(new Entry { handler = handler, priority = priority });
            handlers.Sort((a, b) => b.priority.CompareTo(a.priority));
        }

        public void RemoveHandler(InputCommandHandler handler)
        {
            handlers.RemoveAll(e => e.handler == handler);
        }

        // 커맨드 주입: 이번 프레임 실제 입력 처리 후 동일 경로로 디스패치된다
        public void Simulate(InputCommand command)
        {
            simulated.Add(command);
        }

        void Update()
        {
            PointerDownThisFrame = false;
            PointerUpThisFrame = false;
            BackPressedThisFrame = false;
            PointerHeld = false;

            // --- 1. 원시 입력 → 커맨드
            var pointer = Pointer.current;
            if (pointer != null)
            {
                Vector2 pos = pointer.position.ReadValue();
                PointerPosition = pos; // 레거시 mousePosition처럼 항상 최신 좌표 유지

                if (pointer.press.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.PointerDown, pos));
                if (pointer.press.isPressed)
                    Dispatch(new InputCommand(InputCommandType.PointerHold, pos));
                if (pointer.press.wasReleasedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.PointerUp, pos));
            }

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.escapeKey.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.Back));
                if (keyboard.upArrowKey.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.MoveUp));
                if (keyboard.downArrowKey.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.MoveDown));
                if (keyboard.leftArrowKey.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.MoveLeft));
                if (keyboard.rightArrowKey.wasPressedThisFrame)
                    Dispatch(new InputCommand(InputCommandType.MoveRight));
            }

            // --- 2. 주입된 커맨드
            if (simulated.Count > 0)
            {
                for (int i = 0; i < simulated.Count; i++)
                    Dispatch(simulated[i]);
                simulated.Clear();
            }
        }

        void Dispatch(in InputCommand command)
        {
            // 폴링 뷰 갱신 - 커맨드 스트림이 단일 소스 (Simulate 주입도 동일하게 반영됨)
            switch (command.type)
            {
                case InputCommandType.PointerDown:
                    PointerDownThisFrame = true;
                    PointerHeld = true;
                    PointerPosition = command.pointerPosition;
                    break;
                case InputCommandType.PointerHold:
                    PointerHeld = true;
                    PointerPosition = command.pointerPosition;
                    break;
                case InputCommandType.PointerUp:
                    PointerUpThisFrame = true;
                    PointerHeld = false;
                    PointerPosition = command.pointerPosition;
                    break;
                case InputCommandType.Back:
                    BackPressedThisFrame = true;
                    break;
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i].handler(in command))
                    break; // 소비됨
            }
        }
    }
}

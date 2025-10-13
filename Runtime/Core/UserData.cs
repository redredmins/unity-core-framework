using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{

    [Serializable]
    public abstract class Data
    {
        public virtual void SaveData() { }
        public virtual void LoadData() { }
    }

    public class UserData : SingletonMonobehaviour<UserData>
    {
        protected Dictionary<string, Data> _datas;


        protected override void OnSingletonAwake()
        {
            _datas = new Dictionary<string, Data>();
        }

        public void RegisterData(string dataType, Data data)
        {
            if (_datas.ContainsKey(dataType) == true)
                _datas[dataType] = data;
            else
                _datas.Add(dataType, data);
        }

        public Data GetUserData(string dataType)
        {
            if (_datas.ContainsKey(dataType) == true)
                return _datas[dataType];
            else
                return null;
        }
    }
}

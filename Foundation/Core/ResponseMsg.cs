using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web.Http.ModelBinding;

namespace Foundation.Core
{

    public struct ErrorMsg {
        public ErrorMsg(String key,String value)
        {
            Key = key;
            Value = value;
        }
        public String Key { get; set; }
        public String Value { get; set; }
    }

    public struct DataItem
    {
        public DataItem(String key, String value)
        {
            Key = key;
            Value = value;
        }
        public String Key { get; set; }
        public String Value { get; set; }
    }
    public class ResponseMsg<T>
    {
        public string Code { get; set; }
        public String Message { get; set; }
        public T Content { get; set; }
        public List<ErrorMsg> Errors { get; set; }
        public long TotalCount { get; set; }
            
        public ResponseMsg()
        {
            Errors = new List<ErrorMsg>();
        }
        public ResponseMsg(ModelStateDictionary modelState)
        {
            Errors = new List<ErrorMsg>();
            foreach (string key in modelState.Keys)
            {
                if (modelState[key].Errors.Count() > 0)
                {
                    foreach (var error in modelState[key].Errors)
                    {
                        Errors.Add(new ErrorMsg(key, error.ErrorMessage));
                    }
                }
            }
        }
    }

}


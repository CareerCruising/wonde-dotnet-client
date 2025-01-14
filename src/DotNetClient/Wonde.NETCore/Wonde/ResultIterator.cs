﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Wonde.EndPoints;
using Wonde.Helpers;
using Wonde.Wonde;
using static System.Net.Mime.MediaTypeNames;

namespace Wonde
{

    /// <summary>
    /// Json data received represented as Array
    /// </summary>
    public class ResultIterator : BootstrapEndpoint, IEnumerable, IEnumerator
    {

        private IEnumerator arrEnum;

        /// <summary>
        /// Returns the data as ArrayList
        /// </summary>
        public IEnumerable ArrayData { get; private set; }

        /// <summary>
        /// Returns the Meta information as Dictionary
        /// </summary>
        public Dictionary<string, object> MetaData { get; private set; }

        /// <summary>
        /// Returns the token used
        /// </summary>
        public new string Token { get; private set; }

        /// <summary>
        /// Gets the count of the current Array
        /// </summary>
        /// <returns>Array length in integer</returns>
        public int Count
        {
            get
            {
                var objList = ArrayData as ArrayList;
                var objDict = ArrayData as Dictionary<string, object>;
                if (objList != null)
                    return objList.Count;
                else if (objDict != null)
                    return objDict.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Constructor to create ResultIterator object
        /// </summary>
        /// <param name="resp">Data returned in Json converted to Dictionary as Key/Value</param>
        /// <param name="token">Token used</param>
        public ResultIterator(Dictionary<string, object> resp, string token) : base(token, "")
        {
            if (resp.TryGetValue("data", out var dataObj))
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new ObjectToArrayConverter() } // Custom converter for handling objects as arrays
                };
                ArrayData = JsonSerializer.Deserialize<IEnumerable<object>>(dataObj.ToString(), options);
            }

            if (resp.TryGetValue("meta", out var metaObj) && metaObj is JsonElement dataElement)
            {
                MetaData = JsonSerializer.Deserialize<Dictionary<string, object>>(dataElement.GetRawText());
            }

            this.Token = token;
        }

        /// <summary>
        /// Gets the enumerator for the object
        /// </summary>
        /// <returns>IEnumerator for the array</returns>
        public IEnumerator GetEnumerator()
        {
            arrEnum = ArrayData.GetEnumerator();
            return this;
        }

        /// <summary>
        /// Revinds the cursor to its initial position
        /// </summary>
        public void Reset()
        {
            arrEnum.Reset();
        }

        /// <summary>
        /// Returns the current key of the cursor position
        /// </summary>
        /// <returns>Current position</returns>
        public int key()
        {
            var arrList = ArrayData as ArrayList;

            if (arrList != null)
                return arrList.IndexOf(arrEnum.Current);
            else
                return -1;
        }

        /// <summary>
        /// returns the current object in the array as per cursor position
        /// </summary>
        /// <returns>Dictionary&ltstring, object&gt object at current position in array</returns>
        public object Current
        {
            get
            {
                return arrEnum.Current;
            }
        }

        /// <summary>
        /// Returns the next object and moves the cursor
        /// </summary>
        /// <returns>The next object in the array moving the cursor as well</returns>
        public bool MoveNext()
        {
            if (arrEnum.MoveNext())
                return true;
            else
            {
                if (nextPage())
                    return arrEnum.MoveNext();
                else
                    return false;
            }
        }

        /// <summary>
        /// Loads the next page from the api server, if available
        /// </summary>
        /// <returns>true if next page is loaded successfully else false</returns>
        public bool nextPage()
        {
            var pages = JsonSerializer.Deserialize<Dictionary<string, object>>(MetaData["pagination"].ToString());

            if (pages["next"] == null)
                return false;

            var next = pages["next"].ToString();
            return processUrl(next);
        }

        /// <summary>
        /// Loads the previous page from the api server, if available
        /// </summary>
        /// <returns>true if previous is loaded else false</returns>
        public bool previousPage()
        {
            var pages = JsonSerializer.Deserialize<Dictionary<string, object>>(MetaData["pagination"].ToString());
            if (pages["previous"] == null)
                return false;

            var previous = pages["previous"].ToString();
            return processUrl(previous);
        }

        /// <summary>
        /// Private function to get data from the server
        /// </summary>
        /// <param name="url">Url to call</param>
        /// <returns>True if success</returns>
        private bool processUrl(string url)
        {
            if (url.Trim().Length == 0)
                return false;

            var res = StringHelper.getJsonAsDictionary(getUrl(url));
            if (res == null)
                return false;

            MetaData = JsonSerializer.Deserialize<Dictionary<string, object>>(res["meta"].ToString());
            ArrayData = JsonSerializer.Deserialize<IEnumerable<object>>(res["data"].ToString());

            Reset();
            arrEnum = ArrayData.GetEnumerator();
            return true;
        }
    }
}

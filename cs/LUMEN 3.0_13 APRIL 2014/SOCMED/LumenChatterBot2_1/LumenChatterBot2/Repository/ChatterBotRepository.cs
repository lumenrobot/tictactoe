using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LumenChatterBot2.Model;

namespace LumenChatterBot2.Repository
{
    class ChatterBotRepository
    {
        #region global variable
        //**************************
        //public static string PreviousResponse = "";

        //**************************
        #endregion

        //Find relliable response based on keyword
        public static string FindMatchKeyWord(string input, List<int> responHistory, out int botResponId, out bool isResponRepeated)
        {
            isResponRepeated = false;
            botResponId = 0;
            var result = "";
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var responseDb = (from a in dc.vw_KeyWordResponses where a.KeyWord == input select a).ToList();

                var sameResponList = new List<int>();

                //prevent same respon with previous
                foreach (var respon in responseDb)
                {
                    int id = 0;
                    id = (from a in responHistory where a == respon.Id select a).FirstOrDefault();
                    if (id != 0)
                    {
                        sameResponList.Add(id);
                        if (!isResponRepeated)
                        {
                            isResponRepeated = true;
                        }
                    }
                }

                if (sameResponList.Count > 0)
                {
                    foreach (var i in sameResponList)
                    {
                        var itemToRemove = responseDb.FirstOrDefault(m => m.Id == i);
                        if (itemToRemove != null)
                        {
                            responseDb.Remove(itemToRemove);
                        }
                    }
                }

                if (responseDb.Count > 0)
                {
                    //get unique response 
                    var random = new Random();
                    int responseIndex = random.Next(0, responseDb.Count - 1);
                    var responChosen = responseDb[responseIndex];
                    result = responChosen.Response;
                    botResponId = responChosen.Id;
                }
                else
                {
                    //do nothing
                }
            }
            catch (Exception)
            {
                //todo : implement exception
            }
            return result;
        }

        //remove white-space
        //set to lowercase
        public static string PrepareInput(string input)
        {
            input = input.ToLower();
            input = Regex.Replace(input, @"[^a-z0-9 ]", string.Empty);
            input = input.Trim();
            return input;
        }


    }
}

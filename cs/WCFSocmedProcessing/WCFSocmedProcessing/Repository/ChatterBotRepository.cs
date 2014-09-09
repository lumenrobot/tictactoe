using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using WCFSocmedProcessing.Model;

namespace WCFSocmedProcessing.Repository
{
    class ChatterBotRepository
    {
        #region global variable
        //**************************
        //public static string PreviousResponse = "";

        //**************************
        #endregion

        #region OLD CODE - PITY TO DELETE
        /*
        //Find relliable response based on keyword; 4 parameters
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

        public static string FindMatchKeyWord(string input)
        {
            var result = "";
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var responseDb = (from a in dc.vw_KeyWordResponses where a.KeyWord == input select a).ToList();

                if (responseDb.Count > 0)
                {
                    //get unique response 
                    var random = new Random();
                    int responseIndex = random.Next(0, responseDb.Count - 1);
                    var responChosen = responseDb[responseIndex];
                    result = responChosen.Response;
                }
                else
                {
                    StackFrame callStack = new StackFrame(1, true);
                    result = "<Not Define >> Check file : "+ callStack.GetFileName() +", Line : " + callStack.GetFileLineNumber() + ">";
                }
            }
            catch (Exception)
            {
                //todo : implement exception
                result = "<Error Server>";
            }
            return result;
        }

        public static Response FindResponse(string input)
        {
            var model = new Response{Behaviour = "0",Text = "<no inputText>"};
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var responseDb = (from a in dc.vw_KeyWordResponses where a.KeyWord == input select a).ToList();

                if (responseDb.Count > 0)
                {
                    //get unique response 
                    var random = new Random();
                    int responseIndex = random.Next(0, responseDb.Count - 1);
                    var responChosen = responseDb[responseIndex];
                    model.Text = responChosen.Response;
                    model.Behaviour = responChosen.Behaviour;
                }
                else
                {
                    //todo: implement algorithm if there is no response in db

                    StackFrame callStack = new StackFrame(1, true);
                    model.Text = "<Find No Respon, Check Line : " + callStack.GetFileLineNumber() + ">";

                }
            }
            catch (Exception)
            {
                //todo : implement exception
                model.Text = "<Error Server>";
            }
            return model;
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

        public static List<string> LoadGrammar()
        {
            var grammar = new List<string>();
            try
            {
                var dc = new LumenChatterBotDBDataContext();
                var responseDb = (from a in dc.KeyWordTbls select a).ToList();

                if (responseDb.Any())
                {
                    foreach (var m in responseDb)
                    {
                        grammar.Add(m.KeyWord);
                    }
                }
                else
                {
                    grammar.Add("No grammar loaded.");
                }
            }
            catch (Exception ex)
            {
                grammar.Add("Server Error." + ex.Message);
            }
            return grammar;
        }

        //*/
        #endregion
    }
}

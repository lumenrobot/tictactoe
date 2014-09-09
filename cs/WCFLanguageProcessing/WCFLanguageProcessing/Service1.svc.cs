using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCFLanguageProcessing.Model;
using WCFLanguageProcessing.Repository;

namespace WCFLanguageProcessing
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        private static string personName = "";

        public void SetPersonName(string name)
        {
            personName = name;
        }

        public string GetPersonName()
        {
            return personName;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetTextResponse(string textIn)
        {
            var response = "<no input>";
            if(!string.IsNullOrEmpty(textIn))
            {
                response = ChatterBotRepository.FindMatchKeyWord(textIn);
                
            }
            return response;
        }

        public Response GetResponse (string textIn)
        {
            var response = new Response{Behaviour = "0",Text = "<no input>"};
            if (!string.IsNullOrEmpty(textIn))
                response = ChatterBotRepository.FindResponse(textIn);
            if(personName.Any())
            {
                if(response.Behaviour == "1" || response.Behaviour=="2" || response.Behaviour=="3")
                {
                    response.Text = response.Text + " " + personName;
                }
            }
            return response;
        }

        public List<string> LoadGrammar()
        {
            var grammar = new List<string>();

            grammar = ChatterBotRepository.LoadGrammar();

            return grammar;
        } 

    }
}

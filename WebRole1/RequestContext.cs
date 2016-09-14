using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class OmniRequestContext
    {
        public string id { get; set; }

        public OmniRequestContext()
        {

        }

        private static OmniRequestContext unitTestRequestContext;
        public static OmniRequestContext Current
        {
            get
            {
                if (unitTestRequestContext == null)
                {
                    return HttpContext.Current.Items["RequestContext"] as OmniRequestContext;
                }
                else
                {
                    return unitTestRequestContext;
                }
            }

            set
            {
                // Allowed only for unit test
                unitTestRequestContext = value;
            }
        }

        public static void InitializeRequestContext()
        {
            HttpContext.Current.Items.Add("RequestContext", new OmniRequestContext());
        }

        public static void InitializeUnitTestRequestContext()
        {
            unitTestRequestContext = new OmniRequestContext();
        }
    }
}
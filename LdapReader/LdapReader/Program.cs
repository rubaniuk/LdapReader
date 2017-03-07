using System;
using System.DirectoryServices;

namespace LdapReader
{
    /// <summary>
    /// Sample for LDAP usage in C#. Looks up a name, and then 
    /// </summary>
    class Program
    {

        static void PrintUser(SearchResult result, int indentation)
        {
            string tabs = string.Empty;
            for (int i = 0; i < indentation; i++)
            {
                tabs += "\t";
            }

            if (result.Properties["displayname"].Count > 0)
            {
                Console.WriteLine("{0}Display name: {1}", tabs, result.Properties["displayname"][0].ToString());
            }
            if (result.Properties["mail"].Count > 0)
            {
                Console.WriteLine("{0}Email: {1}", tabs, result.Properties["mail"][0].ToString());
            }
            if (result.Properties["title"].Count > 0)
            {
                Console.WriteLine("{0}Title: {1}", tabs, result.Properties["title"][0].ToString());
            }
            if (result.Properties["telephonenumber"].Count > 0)
            {
                Console.WriteLine("{0}Office Phone: {1}", tabs, result.Properties["telephonenumber"][0].ToString());
            }
            if (result.Properties["mobile"].Count > 0)
            {
                Console.WriteLine("{0}Mobile Phone: {1}", tabs, result.Properties["mobile"][0].ToString());
            }
            if (result.Properties["homephone"].Count > 0)
            {
                Console.WriteLine("{0}Display name: {1}", tabs, result.Properties["homephone"][0].ToString());
            }
            if (result.Properties["department"].Count > 0)
            {
                Console.WriteLine("{0}Department: {1}", tabs, result.Properties["department"][0].ToString());
            }
            if (result.Properties["manager"].Count > 0)
            {
                Console.WriteLine("{0}Manager: {1}", tabs, result.Properties["manager"][0].ToString());
            }

            //if (result.Properties["DirectReports"].Count > 0)
            //{
            //    foreach (var str in result.Properties["DirectReports"])
            //    {
            //        Console.WriteLine("{0}Direct Report: {1}", tabs, str.ToString());
            //    }
            //}

            if (result.Properties["distinguishedName"].Count > 0)
            {
                try
                {
                    SearchResultCollection directReports = UserDirectReports(result.Properties["distinguishedName"][0].ToString());
                    foreach (SearchResult user in directReports)
                    {
                        PrintUser(user, indentation + 1);
                        Console.WriteLine();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("No reports for: " + result.Properties["distinguishedName"][0].ToString());
                }
            }
        }

        static SearchResultCollection UserDirectReports(string distinguishedName)
        {
            SearchResultCollection results = null;

            DirectorySearcher ds = new DirectorySearcher();
            ds.PropertiesToLoad.Add("displayName");
            ds.PropertiesToLoad.Add("telephoneNumber");
            ds.PropertiesToLoad.Add("mobile");
            ds.PropertiesToLoad.Add("homephone");
            ds.PropertiesToLoad.Add("mail");
            ds.PropertiesToLoad.Add("title");
            ds.PropertiesToLoad.Add("department");
            ds.PropertiesToLoad.Add("manager");
            ds.PropertiesToLoad.Add("distinguishedName");
            ds.PropertiesToLoad.Add("directReports");

            ds.Filter = string.Format("(&(objectCategory=user)(manager={0}))", distinguishedName);

            try
            {
                results = ds.FindAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No reports for: " + distinguishedName);
            }

            return results;
        }


        static void PrintUser(SearchResult result, string[] properties, int indentation)
        {
            if (result == null || properties == null) { return; }

            foreach (var property in properties)
            {
                string tabs = string.Empty;
                for (int i = 0; i < indentation; i++)
                {
                    tabs += "\t";
                }

                if (result.Properties[property].Count > 0)
                {
                    Console.WriteLine("{0}{1}: {2}", tabs, property, result.Properties[property][0].ToString());
                }
            }
        }

        /// <summary>
        /// Prints out various interesting user properties from ActiveDirectory
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: ldapquery alias1 alias2...");
                return;
            }

            string[] properties = {
                "displayName",
                "telephoneNumber",
                "mobile",
                "homephone",
                "mail",
                "title",
                "department",
                "manager",

                "distinguishedName",
                "directReports"
            };

            DirectorySearcher ds = new DirectorySearcher();
            foreach (var property in properties)
            {
                ds.PropertiesToLoad.Add(property);
            }

            foreach (string alias in args)
            {
                ds.Filter = "(SAMAccountName=" + alias + ")";
                SearchResult result = ds.FindOne();

                if (result == null)
                {
                    Console.Error.WriteLine("Could not resolve {0}", alias);
                    continue;
                }

                Console.WriteLine("{0}:", alias);
                PrintUser(result, 0);

                Console.WriteLine();
            }
        }
    }
}
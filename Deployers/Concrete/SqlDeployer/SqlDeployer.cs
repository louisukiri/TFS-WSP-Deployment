using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using toDev.Abstract;
using toDev.DirectoryHelpers;

namespace Deployers.Concrete.SqlDeployer
{
    public class SqlDeployer : FileDeployment
    {
        public string ConnectionString { get; set; }
        public SqlDeployer(string connStr)
        {
            ConnectionString = connStr;
        }
        public override IDirectoryHelper Dir
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                
            }
        }
        public override string Deploy(IList<IFileObject> fobjs)
        {                        
            string outputMsg = string.Empty;
            foreach (IFileObject fobj in fobjs)
            {
                using (Stream stream = fobj.FileStream)
                {
                    
                    if (stream != null)
                    {
                        StreamReader strR = new StreamReader(stream);
                        string sql = strR.ReadToEnd();
                        sql = Regex.Replace(sql, @"(\\r|\\n|\bGO\b)+", "");
                        List<string> SqlSegments = Regex.Split(sql, "\bGO\b")
                            .Select(a=> Regex.Replace(a, @"(\\r|\\n)+", ""))
                            .ToList();
                        //string[] sqlArr = sql.Split(new string[] { "\r","\n","GO" }, StringSplitOptions.RemoveEmptyEntries);
                        int segCount = 1;
                        foreach (string execCommand in SqlSegments)
                        {
                            if(!string.IsNullOrWhiteSpace(execCommand))
                                using (SqlConnection conn = new SqlConnection(ConnectionString))
                                {
                                    try
                                    {
                                        SqlCommand cmd = new SqlCommand(execCommand);
                                        cmd.Connection = conn;
                                        conn.Open();
                                        cmd.ExecuteNonQuery();
                                        conn.Close();
                                        outputMsg += string.Format("     \tsuccessfully executed segment {1} of {2} in {0} \r\n", fobj.FullFileName, segCount.ToString(), SqlSegments.Count.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        outputMsg += string.Format("Error deploying segment {2} of {3} in {0}\r\nError:     {1} \r\n", fobj.FullFileName , ex.Message, segCount.ToString(), SqlSegments.Count.ToString());
                                    }
                                }
                                segCount++;
                        }
                    }
                }
            }
                        string mFileName = string.Format("{0}/{1}.{2}",this.DeploymentDir, this.Tag, "txt");

                        return "\r\n\r\nBuild " + BuildNumber + "(" + Tag + ") SQL Deployment: \r\n" + outputMsg;
                        //File.WriteAllText(mFileName, "SQL for Build" + BuildNumber + "("+ Tag +"): \r\n" + outputMsg);

        }
    }
}

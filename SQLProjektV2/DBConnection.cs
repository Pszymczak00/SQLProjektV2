using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2
{
    public static class DBConnection
    {
        private static string connectionString = "Data Source=LAPTOP-48S3MQBQ;Initial Catalog=Firma;User ID=User;Password=Pass1234;Integrated Security=False;";

        public static DataTable Basic(string procName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var da = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    da.Fill(table);
                    return table;
                }
            }
        }


        public static DataTable BasicId(string procName, int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                using (var da = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    da.Fill(table);
                    return table;
                }
            }
        }

        public static Dictionary<int, string> GetDict(string procName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var da = new SqlDataAdapter(cmd))
                {
                    Dictionary<int, string> temp = new Dictionary<int, string>();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            temp.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                    return temp;
                }
            }
        }

        public static void SQLCommand(string polecenie)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = polecenie;
                cmd.Connection = conn;

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public static int SQLCommandRet(string polecenie)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = polecenie;
                cmd.Connection = conn;

                conn.Open();

                int temp;
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        temp = dr.GetInt32(0);
                    }
                    else temp = 1;
                }

                conn.Close();
                return temp;
            }
        }


        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }






    }
}

using System.Data;
using MySql.Data.MySqlClient;

public class DBHelper
{
    private string connectionString =
        "Server=localhost;Database=MyPC;UID=root;Pwd=";
    public static MySqlConnection GetConnection()
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        try
        {
            conn.Open();
            return conn;
        }
        catch(MySqlException ex)
        {
            System.Windows.Forms.MessageBox.Show("Loi ket noi CSDL: " + ex.Message);
            return null;
        }
    }
    public static DataTable ExecuteQuery(string query)
    {
        using (MySqlConnection conn = GetConnection())
        {
            if (conn == null) return null;
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
    public static int ExecuteNonQuery(string query)
    {
        using (MySqlConnection conn = GetConnection())
        {
            if (conn == null) return 0;
            MySqlCommand cmd = new MySqlCommand(query, conn);
            return cmd.ExecuteNonQuery();
        }
    }
}

using DDD_program;
using System;
using System.Data.SQLite;

public static class SystemLogger
{
    private static string _currentUser = "";
    private static string _currentRole = "";
    private static int _currentLogId = -1;

    // -------------------------
    // LOGIN EVENT
    // -------------------------
    public static void LogLogin(string username, string role)
    {
        _currentUser = username;
        _currentRole = role;

        using var conn = SQLstorage.GetConnection();
        conn.Open();

        using var cmd = new SQLiteCommand(@"
            INSERT INTO Logs (Username, Role, LoginTime, ActionsNoted)
            VALUES (@u, @r, @time, '')
        ", conn);

        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@r", role);
        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();

        using var getId = new SQLiteCommand("SELECT last_insert_rowid()", conn);
        _currentLogId = Convert.ToInt32(getId.ExecuteScalar());
    }

    // -------------------------
    // ACTION EVENT
    // -------------------------
    public static void LogAction(string action)
    {
        if (_currentLogId == -1) return;

        using var conn = SQLstorage.GetConnection();
        conn.Open();

        using var cmd = new SQLiteCommand(@"
            UPDATE Logs
            SET ActionsNoted = ActionsNoted || @entry
            WHERE LogID = @id
        ", conn);

        string entry = $"{DateTime.Now:HH:mm:ss} - {action}\n";

        cmd.Parameters.AddWithValue("@entry", entry);
        cmd.Parameters.AddWithValue("@id", _currentLogId);
        cmd.ExecuteNonQuery();
    }

    // -------------------------
    // LOGOUT EVENT
    // -------------------------
    public static void LogLogout()
    {
        if (_currentLogId == -1) return;

        using var conn = SQLstorage.GetConnection();
        conn.Open();

        using var cmd = new SQLiteCommand(@"
            UPDATE Logs
            SET LogoutTime = @time
            WHERE LogID = @id
        ", conn);

        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@id", _currentLogId);
        cmd.ExecuteNonQuery();

        _currentLogId = -1;
    }
}

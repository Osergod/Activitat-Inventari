using System;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    private void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/rpg_inventory.db";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Tabla Usuaris
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Usuaris (
                            UserID      INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username    TEXT    UNIQUE NOT NULL,
                            Password    TEXT    NOT NULL
                        )";
                    cmd.ExecuteNonQuery();

                    // Tabla Item (MaxStack fijo en 99)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Item (
                            ID          INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre      TEXT NOT NULL UNIQUE,
                            Descripcion TEXT,
                            MaxStack    INTEGER NOT NULL DEFAULT 99
                        )";
                    cmd.ExecuteNonQuery();

                    // Tabla Inventario (Cantidad nunca menor que 0)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Inventario (
                            InventarioID INTEGER PRIMARY KEY AUTOINCREMENT,
                            userId       INTEGER NOT NULL,
                            itemId       INTEGER NOT NULL,
                            Cantidad     INTEGER NOT NULL DEFAULT 0 CHECK(Cantidad >= 0),
                            FOREIGN KEY (userId) REFERENCES Usuaris(UserID) ON DELETE CASCADE,
                            FOREIGN KEY (itemId) REFERENCES Item(ID) ON DELETE RESTRICT,
                            UNIQUE(userId, itemId)
                        )";
                    cmd.ExecuteNonQuery();

                    // Insertar items con MaxStack 99
                    cmd.CommandText = @"
                        INSERT OR IGNORE INTO Item (Nombre, Descripcion, MaxStack) VALUES
                        ('Item1', 'Descripción del Item 1', 99),
                        ('Item2', 'Descripción del Item 2', 99),
                        ('Item3', 'Descripción del Item 3', 99),
                        ('Item4', 'Descripción del Item 4', 99);
                    ";
                    cmd.ExecuteNonQuery();
                }
            }

            Debug.Log("Base de datos inicializada en: " + dbPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error inicializando DB: " + e.Message);
        }
    }

    // ================= LOGIN Y REGISTER =================

    public string RegisterUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "El nombre de usuario no puede estar vacío";

        if (password.Length < 8)
            return "La contraseña debe tener al menos 8 caracteres";

        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Usuaris WHERE Username = @user";
                    cmd.Parameters.AddWithValue("@user", username);

                    long count = (long)cmd.ExecuteScalar();
                    if (count > 0)
                        return "Este usuario ya existe";

                    cmd.CommandText = "INSERT INTO Usuaris (Username, Password) VALUES (@user, @pass)";
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.ExecuteNonQuery();

                    return "OK";
                }
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    public (bool success, string message, int userId) LoginUser(string username, string password)
    {
        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT UserID FROM Usuaris WHERE Username = @user AND Password = @pass";
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                        return (true, "Login correcto", Convert.ToInt32(result));

                    return (false, "Usuario o contraseña incorrectos", -1);
                }
            }
        }
        catch (Exception ex)
        {
            return (false, "Error de conexión: " + ex.Message, -1);
        }
    }

    // ================= INVENTARIO =================

    public bool AddItem(int userId, int itemId)
    {
        int actual = GetCantidad(userId, itemId);

        if (actual >= 99) // máximo fijo
            return false;

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Inventario (userId, itemId, Cantidad)
                    VALUES (@uid, @iid, 1)
                    ON CONFLICT(userId, itemId)
                    DO UPDATE SET Cantidad = MIN(Cantidad + 1, 99)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@iid", itemId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool RestarItem(int userId, int itemId)
    {
        int actual = GetCantidad(userId, itemId);

        if (actual <= 0) // mínimo fijo
            return false;

        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    UPDATE Inventario
                    SET Cantidad = MAX(Cantidad - 1, 0)
                    WHERE userId = @uid AND itemId = @iid";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@iid", itemId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public int GetCantidad(int userId, int itemId)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Cantidad FROM Inventario WHERE userId = @uid AND itemId = @iid";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@iid", itemId);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}
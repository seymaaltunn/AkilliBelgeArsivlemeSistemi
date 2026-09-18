using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace BelgeArsivlemeSistemi.Database
{
    public static class DatabaseInitializer
    {
        private const int CurrentDatabaseVersion = 1;

        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "belgearsiv.db");

        public static string ConnectionString =>
            $"Data Source={DatabasePath};Foreign Keys=True";

        public static void Initialize()
        {
            using var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            CreateTable(connection);
            ApplyMigrations(connection);
            CreateIndexes(connection);
        }

        private static void CreateTable(SqliteConnection connection)
        {
            const string sql = """
                CREATE TABLE IF NOT EXISTS Belgeler
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    BelgeAdi TEXT NOT NULL,
                    BelgeTuru TEXT NOT NULL,
                    KisiKurum TEXT,
                    KimlikNo TEXT,
                    Kurum TEXT,
                    BelgeTarihi TEXT,
                    EvrakNo TEXT,
                    Aciklama TEXT,
                    DosyaYolu TEXT NOT NULL,
                    OcrMetni TEXT,
                    KayitTarihi TEXT NOT NULL,
                    GuncellemeTarihi TEXT
                );
                """;

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static void CreateIndexes(SqliteConnection connection)
        {
            const string sql = """
                CREATE INDEX IF NOT EXISTS IX_Belgeler_BelgeAdi
                    ON Belgeler(BelgeAdi);

                CREATE INDEX IF NOT EXISTS IX_Belgeler_BelgeTuru
                    ON Belgeler(BelgeTuru);

                CREATE INDEX IF NOT EXISTS IX_Belgeler_KisiKurum
                    ON Belgeler(KisiKurum);

                CREATE INDEX IF NOT EXISTS IX_Belgeler_EvrakNo
                    ON Belgeler(EvrakNo);

                CREATE INDEX IF NOT EXISTS IX_Belgeler_KayitTarihi
                    ON Belgeler(KayitTarihi);
                """;

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static void ApplyMigrations(SqliteConnection connection)
        {
            int version = GetDatabaseVersion(connection);

            if (version < 1)
            {
                using var transaction = connection.BeginTransaction();

                AddColumnIfMissing(connection, transaction, "KimlikNo", "TEXT");
                AddColumnIfMissing(connection, transaction, "Kurum", "TEXT");
                AddColumnIfMissing(connection, transaction, "BelgeTarihi", "TEXT");
                AddColumnIfMissing(connection, transaction, "EvrakNo", "TEXT");
                AddColumnIfMissing(connection, transaction, "GuncellemeTarihi", "TEXT");

                SetDatabaseVersion(
                    connection,
                    transaction,
                    CurrentDatabaseVersion);

                transaction.Commit();
            }
        }

        private static void AddColumnIfMissing(
            SqliteConnection connection,
            SqliteTransaction transaction,
            string columnName,
            string columnDefinition)
        {
            if (ColumnExists(connection, transaction, columnName))
                return;

            string sql =
                $"ALTER TABLE Belgeler ADD COLUMN {columnName} {columnDefinition};";

            using var command = new SqliteCommand(sql, connection, transaction);
            command.ExecuteNonQuery();
        }

        private static bool ColumnExists(
            SqliteConnection connection,
            SqliteTransaction transaction,
            string columnName)
        {
            const string sql = "PRAGMA table_info(Belgeler);";

            using var command = new SqliteCommand(sql, connection, transaction);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string existingColumnName = reader.GetString(1);

                if (string.Equals(
                        existingColumnName,
                        columnName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetDatabaseVersion(SqliteConnection connection)
        {
            using var command =
                new SqliteCommand("PRAGMA user_version;", connection);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        private static void SetDatabaseVersion(
            SqliteConnection connection,
            SqliteTransaction transaction,
            int version)
        {
            using var command = new SqliteCommand(
                $"PRAGMA user_version = {version};",
                connection,
                transaction);

            command.ExecuteNonQuery();
        }
    }
}

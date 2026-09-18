using BelgeArsivlemeSistemi.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BelgeArsivlemeSistemi.Database
{
    public class DatabaseService
    {

        public List<DocumentRecord> BelgeAra(
    string belgeAdi,
    string belgeTuru,
    string kisiKurum,
    DateTime baslangic,
    DateTime bitis)
        {
            var belgeler = new List<DocumentRecord>();

            using var connection =
                new SqliteConnection(DatabaseInitializer.ConnectionString);

            connection.Open();

            const string sql = """
        SELECT
            Id,
            BelgeAdi,
            BelgeTuru,
            KisiKurum,
            Aciklama,
            DosyaYolu,
            OcrMetni,
            KayitTarihi
        FROM Belgeler
        WHERE
            BelgeAdi LIKE @BelgeAdi
            AND BelgeTuru LIKE @BelgeTuru
            AND KisiKurum LIKE @KisiKurum
            AND datetime(KayitTarihi)
                BETWEEN datetime(@Baslangic)
                AND datetime(@Bitis)
        ORDER BY Id DESC;
        """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@BelgeAdi",
                $"%{belgeAdi.Trim()}%");

            command.Parameters.AddWithValue(
                "@BelgeTuru",
                string.IsNullOrWhiteSpace(belgeTuru)
                    ? "%"
                    : $"%{belgeTuru.Trim()}%");

            command.Parameters.AddWithValue(
                "@KisiKurum",
                $"%{kisiKurum.Trim()}%");

            command.Parameters.AddWithValue(
                "@Baslangic",
                baslangic.Date.ToString("yyyy-MM-dd 00:00:00"));

            command.Parameters.AddWithValue(
                "@Bitis",
                bitis.Date.ToString("yyyy-MM-dd 23:59:59"));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                belgeler.Add(new DocumentRecord
                {
                    Id = reader.GetInt32(0),
                    BelgeAdi = reader.GetString(1),
                    BelgeTuru = reader.GetString(2),
                    KisiKurum = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Aciklama = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    DosyaYolu = reader.GetString(5),
                    OcrMetni = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    KayitTarihi = DateTime.Parse(reader.GetString(7))
                });
            }

            return belgeler;
        }
        public int BelgeEkle(DocumentRecord belge)
        {
            using var connection =
                new SqliteConnection(DatabaseInitializer.ConnectionString);

            connection.Open();

            const string sql = """
                INSERT INTO Belgeler
                (
                    BelgeAdi,
                    BelgeTuru,
                    KisiKurum,
                    Aciklama,
                    DosyaYolu,
                    OcrMetni,
                    KayitTarihi
                )
                VALUES
                (
                    @BelgeAdi,
                    @BelgeTuru,
                    @KisiKurum,
                    @Aciklama,
                    @DosyaYolu,
                    @OcrMetni,
                    @KayitTarihi
                );

                SELECT last_insert_rowid();
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@BelgeAdi",
                belge.BelgeAdi);

            command.Parameters.AddWithValue(
                "@BelgeTuru",
                belge.BelgeTuru);

            command.Parameters.AddWithValue(
                "@KisiKurum",
                belge.KisiKurum);

            command.Parameters.AddWithValue(
                "@Aciklama",
                belge.Aciklama);

            command.Parameters.AddWithValue(
                "@DosyaYolu",
                belge.DosyaYolu);

            command.Parameters.AddWithValue(
                "@OcrMetni",
                belge.OcrMetni);

            command.Parameters.AddWithValue(
                "@KayitTarihi",
                belge.KayitTarihi.ToString("yyyy-MM-dd HH:mm:ss"));

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public List<DocumentRecord> BelgeleriGetir()
        {
            var belgeler = new List<DocumentRecord>();

            using var connection =
                new SqliteConnection(DatabaseInitializer.ConnectionString);

            connection.Open();

            const string sql = @"
        SELECT
            Id,
            BelgeAdi,
            BelgeTuru,
            KisiKurum,
            Aciklama,
            DosyaYolu,
            OcrMetni,
            KayitTarihi
        FROM Belgeler
        ORDER BY KayitTarihi DESC";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                belgeler.Add(new DocumentRecord
                {
                    Id = reader.GetInt32(0),
                    BelgeAdi = reader.GetString(1),
                    BelgeTuru = reader.GetString(2),
                    KisiKurum = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Aciklama = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    DosyaYolu = reader.GetString(5),
                    OcrMetni = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    KayitTarihi = DateTime.Parse(reader.GetString(7))
                });
            }

            return belgeler;
        }

        public bool BelgeSil(int belgeId)
        {
            using var connection =
                new SqliteConnection(DatabaseInitializer.ConnectionString);

            connection.Open();

            const string sql = """
                DELETE FROM Belgeler
                WHERE Id = @Id;
                """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", belgeId);

            return command.ExecuteNonQuery() > 0;
        }


        public bool BelgeGuncelle(DocumentRecord belge)
        {
            using var connection =
                new SqliteConnection(DatabaseInitializer.ConnectionString);

            connection.Open();

            const string sql = """
                UPDATE Belgeler
                SET
                    BelgeAdi = @BelgeAdi,
                    BelgeTuru = @BelgeTuru,
                    KisiKurum = @KisiKurum,
                    Aciklama = @Aciklama
                WHERE Id = @Id;
                """;

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@BelgeAdi", belge.BelgeAdi);
            command.Parameters.AddWithValue("@BelgeTuru", belge.BelgeTuru);
            command.Parameters.AddWithValue("@KisiKurum", belge.KisiKurum);
            command.Parameters.AddWithValue("@Aciklama", belge.Aciklama);
            command.Parameters.AddWithValue("@Id", belge.Id);

            return command.ExecuteNonQuery() > 0;
        }


    }
}
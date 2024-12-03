using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Data;

namespace WebAppZero
{
    // nome file csv: PlasmacMachines.csv
    public class CsvLoad
    {
        private readonly string _csvFilePath;

        public CsvLoad(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        public async Task csvRead()
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            try
            {
                // Crea una nuova DataTable
                DataTable dataTable = new DataTable();

                using (var reader = new StreamReader(_csvFilePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // Leggi l'intestazione del CSV (nomi delle colonne)
                    csv.Read(); // Leggi la prima riga (l'intestazione)
                    csv.ReadHeader(); // Imposta l'intestazione
                    var header = csv.HeaderRecord; // Ottieni l'intestazione

                    if (header == null || header.Length == 0)
                    {
                        throw new Exception("Il CSV non contiene intestazioni valide.");
                    }

                    // Aggiungi le colonne al DataTable
                    foreach (var column in header)
                    {
                        dataTable.Columns.Add(column); // Aggiungi una colonna per ogni intestazione
                    }

                    // Leggi i record dal CSV
                    foreach (var record in csv.GetRecords<MachineModel>())
                    {
                        // Crea una nuova riga per il DataTable
                        DataRow row = dataTable.NewRow();

                        // Popola la riga con i dati dai record fortemente tipizzati
                        foreach (var column in header)
                        {
                            // Usa reflection per ottenere il valore della proprietà corrispondente alla colonna
                            var propertyInfo = record.GetType().GetProperty(column);
                            if (propertyInfo != null)
                            {
                                var value = propertyInfo.GetValue(record);
                                row[column] = value ?? DBNull.Value; // Gestisci valori nulli
                            }
                            else
                            {
                                row[column] = DBNull.Value; // Gestisci la colonna mancante nella classe
                            }
                        }

                        // Aggiungi la riga al DataTable
                        dataTable.Rows.Add(row);
                    }
                }

                // Ora dataTable contiene tutti i dati letti dal CSV
                // Puoi usarlo come preferisci (ad esempio, per visualizzarlo in una DataGridView, fare altre elaborazioni, ecc.)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }


        }
    }
}   

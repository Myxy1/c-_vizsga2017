using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace vizsga2017_Merenyi_Miklos_14S
{
    public partial class Form3 : Form
    {
        MySqlConnection kapcsolat;
        private DataTable eredmenytabla;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                string kapcsolatString = "SERVER=localhost;" + "DATABASE=befektetes;" + "UID=root;" + "PASSWORD=;";
                kapcsolat = new MySqlConnection(kapcsolatString);
                kapcsolat.Open();
                lista(0);
                SzakteruletFeltoltes();
                dataGridView1.AllowUserToAddRows = false;
                string[] feliratok = { "Név", "Szakterület", "Óradíj", "Telefonszám", "E-mail cím" };
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].HeaderText = feliratok[i];
                }
                /*formázás*/
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Format = "####Ft";
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Probléma az adatbázis megnyitásakor! " + ex.Message);
                Close();
            }
        }

        private void lista(int oldal)
        {
            String parancsStr = "";
            try
            {
                parancsStr = "SELECT " +
                            "tanacsado.nev, szakterulet.megnevezes, tanacsado.oradij, tanacsado.telefon, tanacsado.email " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " + 
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id";
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);
                dataGridView1.DataSource = eredmenytabla;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatok betöltésekor! " + ex + " " + parancsStr);
            }
        }

        private void SzakteruletFeltoltes()
        {
            try
            {                 //Szakterületek előállítása                 
                String parancsStr = "SELECT " +
                            "szakterulet.megnevezes as szakterulet " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " +
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id " +
                            "GROUP BY szakterulet";
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);
                //A Szakterületek betöltése a combobox-ba      
                MySqlDataReader reader = parancs.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["szakterulet"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a nevek feltöltésekor! " + ex.Message);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Minimum = numericUpDown1.Value;
        }

        private void szures(String tantargy, int alsohatar, int felsohatar)
        {
            String parancsStr = "";
            try
            {
                parancsStr = "SELECT " +
                            "tanacsado.nev, szakterulet.megnevezes, tanacsado.oradij, tanacsado.telefon, tanacsado.email " +
                            "FROM " +
                            "szakterulet, tanacsado, talalkozo, ugyfel " +
                            "WHERE " +
                            "szakterulet.szakterulet_id = tanacsado.szakterulet_id AND " +
                            "tanacsado.tanacsado_id = talalkozo.tanacsado_id AND " +
                            "talalkozo.ugyfel_id = ugyfel.ugyfel_id AND " +
                            "szakterulet.megnevezes = '" + comboBox1.Text + "' AND tanacsado.oradij BETWEEN '" + numericUpDown1.Value + "' AND '" + numericUpDown2.Value + "' " +
                            "GROUP BY " +
                            "tanacsado.nev";
                // A legkérdezés végrehajtása, és az eredménytáblába töltése        
                MySqlCommand parancs = new MySqlCommand(parancsStr, kapcsolat);
                eredmenytabla = new DataTable();
                new MySqlDataAdapter(parancs).Fill(eredmenytabla);
                //A lekérdezés eredményének a megjelenítése a datagridview-n           
                dataGridView1.DataSource = eredmenytabla;
                if (dataGridView1.Rows.Count.Equals(0))
                {
                    MessageBox.Show("Nincs ilyen tanácsadó");
                    button3.Enabled = false;
                }
                else
                {
                    button3.Enabled = true;
                }
            }
            catch (Exception ex) 
                {
                MessageBox.Show("Hiba az adatok betöltésekor! " + ex + " " + parancsStr);
                }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            szures(comboBox1.Text, Decimal.ToInt32(numericUpDown1.Value), Decimal.ToInt32(numericUpDown2.Value));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //Excel objektumok létrehozása
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlApp.DisplayAlerts = false;

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    xlWorkSheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                }
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        xlWorkSheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                    }
                }

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    ((Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[i + 2, 8]).NumberFormat = "## 000 Ft";
                }

                Directory.CreateDirectory("c:\\befektetes_reszvenyek");
                DateTime ma = DateTime.Now;
                String filenev = "c:\\befektetes_reszvenyek\\befektetes_reszvenyek_(" + numericUpDown1.Value + "-" + numericUpDown2.Value + " Ft_" + dataGridView1.Rows.Count + ")_" + ma.Year + "-" + ma.Month + "-" + ma.Day + "_" + ma.Hour + "." + ma.Minute + "." + ma.Second + ".csv";
                if (!File.Exists(filenev))
                {
                    xlWorkBook.SaveAs(filenev);
                }
                else
                {
                    if (MessageBox.Show("Van már ilyen fájl felülírjam?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        xlWorkBook.SaveCopyAs(filenev);
                    }
                }
                xlWorkBook.Close();
                xlApp.Quit();
                MessageBox.Show("Ezen az elérési úton találod meg:" + Environment.NewLine + "c:\\befektetes_reszvenyek\\");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az exportálás során!" + ex);
            }

        }
    }
}

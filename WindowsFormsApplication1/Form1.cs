using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString))
            {
                sqlConn.Open();
                var sql = @"SELECT TOP 5 
                            sp.ProductName, 
                            SUM(sp.QtySold) AS QtySold,
                            p.Quantity,
                            CAST((CAST(SUM(sp.QtySold) AS FLOAT) / CAST(p.Quantity AS FLOAT)) * 100 AS DECIMAL(8,2)) [SalesPercentage]
                        FROM 
                            Sales_productholder sp
                            JOIN Products p ON (sp.ProductID = p.ProductID)
                        GROUP BY 
                            sp.ProductName, p.ProductID, p.Quantity
                        ORDER BY 
                            SUM(sp.QtySold) DESC";
                SqlCommand command = new SqlCommand(sql, sqlConn); //top selling with desc
                SqlDataReader read = command.ExecuteReader();
                while (read.Read())
                {
                    var sold = read["QtySold"].ToString();
                    var percent = read["SalesPercentage"].ToString();
                    this.chart1.Series["QuantitySold"].Points.AddXY(read["ProductName"], read["QtySold"]);
                    //add another series for the sold %
                    this.chart1.Series["QuantityPercentage"].Points.AddXY(read["ProductName"], read["SalesPercentage"]);
                }
            }
        }
    }
}

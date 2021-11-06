using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GiaoDien_N20
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection cnn = null;

        public MainWindow()
        {
            InitializeComponent();
            NgayLap.Text = DateTime.Today.ToString("yyyy-MM-dd");
            ConnStrInput.Text = "Data Source=TRONGTINVDPC;Initial Catalog=Nhom_20_test_1;Integrated Security=True";
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectDB();
                MessageBox.Show("Connection Open!");
                UpdateKhachHangList();
                UpdateSanPhamList();
                UpdateHoaDonList();
                UpdateCTHoaDonList();
                UpdateTongDoanhThuList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateTongDoanhThuList()
        {
            TongDoanhThuList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"
SELECT DATEPART(Year, hd.NgayLap) Nam, DATEPART(Month, hd.NgayLap) Thang, SUM(TongTien) AS TongDoanhThu FROM HoaDon hd GROUP BY DATEPART(Year, hd.NgayLap), DATEPART(Month, hd.NgayLap) ORDER BY Nam, Thang";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int Nam = reader.GetInt32(0);
                        int Thang = reader.GetInt32(1);
                        decimal TongDoanhThu = reader.GetDecimal(2);
                        TongDoanhThuList.Items.Add(new { Nam = Nam, Thang = Thang, TongDoanhThu = TongDoanhThu });
                    }
                }
            }
        }

        private void UpdateCTHoaDonList()
        {
            CTHoaDonList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"SELECT MaHD, MaSP, SoLuong FROM CT_HoaDon";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int MaHD = reader.GetInt32(0);
                        int MaSP = reader.GetInt32(1);
                        int SoLuong = reader.GetInt32(2);
                        CTHoaDonList.Items.Add(new { MaHD = MaHD, MaSP = MaSP, SoLuong = SoLuong });
                    }
                }
            }
        }

        private void UpdateHoaDonList()
        {
            HoaDonList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"SELECT MaHD, MaKH, NgayLap, TongTien FROM HoaDon";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int MaHD = reader.GetInt32(0);
                        int MaKH = reader.GetInt32(1);
                        string NgayLap = reader.GetDateTime(2).ToString("dd-MM-yyyy");
                        decimal TongTien = reader.GetDecimal(3);
                        HoaDonList.Items.Add(new { MaHD = MaHD, MaKH = MaKH, NgayLap = NgayLap, TongTien = TongTien });
                    }
                }
            }
        }

        private void UpdateSanPhamList()
        {
            SanPhamList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"SELECT MaSP, TenSP, Gia FROM SanPham";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int MaSp = reader.GetInt32(0);
                        string TenSP = reader.GetString(1);
                        decimal Gia = reader.GetDecimal(2);
                        SanPhamList.Items.Add(new { MaSp = MaSp, TenSP = TenSP, Gia = Gia });
                    }
                }
            }
        }

        private void UpdateKhachHangList()
        {
            KhachHangList.Items.Clear();

            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"SELECT MaKH, Ho, Ten FROM KhachHang";

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int MaKH = reader.GetInt32(0);
                        string Ho = reader.GetString(1);
                        string Ten = reader.GetString(2);
                        KhachHangList.Items.Add(new { MaKH = MaKH, Ho = Ho, Ten = Ten });
                    }
                }
            }
        }

        private void ConnectDB()
        {
            string connectionString = ConnStrInput.Text;
            cnn = new SqlConnection(connectionString);
            cnn.Open();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cnn != null)
            {
                cnn.Close();
                cnn.Dispose();
            }
        }

        private void ThemHoaDonBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int maKH = int.Parse(MaKD.Text);
                DateTime ngayLap = DateTime.Parse(NgayLap.Text);
                int rowCount = CreateHoaDon(maKH, ngayLap);
                UpdateHoaDonList();
                UpdateTongDoanhThuList();
                MessageBox.Show($"{rowCount} row(s) affected.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int CreateHoaDon(int maKH, DateTime ngayLap)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"INSERT INTO HoaDon(MaKH, NgayLap) VALUES(@MaKH, @NgayLap)";

            command.Parameters.Add("@MaKH", System.Data.SqlDbType.Int).Value = maKH;
            command.Parameters.Add("@NgayLap", System.Data.SqlDbType.DateTime).Value = ngayLap;

            int rowCount = command.ExecuteNonQuery();
            return rowCount;
        }

        private void ThemCTHoaDon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int maHD = int.Parse(MaHD.Text);
                int maSP = int.Parse(MaSP.Text);
                int soLuong = int.Parse(SoLuong.Text);
                int rowCount = CreateCTHoaDon(maHD, maSP, soLuong);
                UpdateCTHoaDonList();
                UpdateHoaDonList();
                UpdateTongDoanhThuList();
                MessageBox.Show($"{rowCount} row(s) affected.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int CreateCTHoaDon(int maHD, int maSP, int soLuong)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = @"INSERT INTO CT_HoaDon(MaHD, MaSP, SoLuong) VALUES(@MaHD, @MaSP, @SoLuong)";

            command.Parameters.Add("@MaHD", System.Data.SqlDbType.Int).Value = maHD;
            command.Parameters.Add("@MaSP", System.Data.SqlDbType.Int).Value = maSP;
            command.Parameters.Add("@SoLuong", System.Data.SqlDbType.Int).Value = soLuong;

            int rowCount = command.ExecuteNonQuery();
            return rowCount;
        }
    }
}

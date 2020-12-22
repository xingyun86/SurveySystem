using System;
using System.Collections.Generic;
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
using MathNet.Numerics.LinearAlgebra.Double;

namespace SurveySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int m_nNum = 24;
        int m_nTestExamCount = 0;
        DenseMatrix m_martix_min = new DenseMatrix(3, 1);
        DenseMatrix m_martix_max = new DenseMatrix(3, 1);
        DenseMatrix m_matrix_d = new DenseMatrix(m_nNum, 3); //24×3矩阵
        DenseMatrix m_matrix_w = new DenseMatrix(1, m_nNum); //1x24矩阵
        List<List<RadioButton>> m_radioButtonList = new List<List<RadioButton>>();
        List<DenseMatrix> m_matrix_list = new List<DenseMatrix>();
        
        public MainWindow()
        {
            for (int i = 0; i < m_nNum; i++)
            {
                m_matrix_w[0,i] = 1.0;
            }
            m_matrix_list.Add(new DenseMatrix(24, 3));
            m_martix_min[0, 0] = 1.0;
            m_martix_min[1, 0] = -1.0;
            m_martix_min[2, 0] = -1.0;
            m_martix_max[0, 0] = 1.0;
            m_martix_max[1, 0] = 1.0;
            m_martix_max[2, 0] = -1.0;
            InitializeComponent();
            InitHeadInfo();
            InitExamInfo();
        }
        private void InitHeadInfo()
        {
            this.titleLabel.Content = "调查问卷系统";
        }
        private void InitExamInfo()
        {
            for (int i = 0; i < m_nNum; i++)
            {
                Panel panel = new StackPanel();
                Label label = new Label();
                Panel subPanel = new WrapPanel();
                RadioButton radioButtonA = new RadioButton();
                RadioButton radioButtonB = new RadioButton();
                RadioButton radioButtonC = new RadioButton();
                label.Content = i.ToString() + ":指标";
                radioButtonA.Name = "A";
                radioButtonA.Content = "A.完全同意";
                radioButtonA.Margin = new Thickness(10);
                radioButtonB.Name = "B";
                radioButtonB.Content = "B.同意";
                radioButtonB.Margin = new Thickness(10);
                radioButtonC.Name = "C";
                radioButtonC.Content = "C.不同意";
                radioButtonC.Margin = new Thickness(10);
                subPanel.Children.Add(radioButtonA);
                subPanel.Children.Add(radioButtonB);
                subPanel.Children.Add(radioButtonC);
                panel.Children.Add(label);
                panel.Children.Add(subPanel);
                this.examListBox.Items.Add(panel);
                m_radioButtonList.Add(new List<RadioButton>() { radioButtonA, radioButtonB, radioButtonC });
            }
        }

        private void commitButton_Click(object sender, RoutedEventArgs e)
        {
            if(m_nTestExamCount >= m_matrix_list.Count)
            {
                m_matrix_list.Add(new DenseMatrix(24, 3));
            }
            for (int nItem = 0; nItem < this.m_radioButtonList.Count; nItem++)
            {
                m_matrix_list[m_nTestExamCount][nItem, 0] = 0.0;
                m_matrix_list[m_nTestExamCount][nItem, 1] = 0.0;
                m_matrix_list[m_nTestExamCount][nItem, 2] = 0.0;
                RadioButton radioButonA = this.m_radioButtonList[nItem][0];
                RadioButton radioButonB = this.m_radioButtonList[nItem][1];
                RadioButton radioButonC = this.m_radioButtonList[nItem][2];
                if (radioButonA.IsChecked == true)
                {
                    m_matrix_list[m_nTestExamCount][nItem, 0] = 1.0;
                }
                else if (radioButonB.IsChecked == true)
                {
                    m_matrix_list[m_nTestExamCount][nItem, 1] = 1.0;
                }
                else if (radioButonC.IsChecked == true)
                {
                    m_matrix_list[m_nTestExamCount][nItem, 2] = 1.0;
                }
                else
                {
                    return;
                }
            }
            m_nTestExamCount++;
            this.titleLabel.Content = "调查问卷系统" + m_nTestExamCount.ToString();
        }
        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < m_nNum; i++)
            {
                double a = 0.0;
                double b = 0.0;
                double c = 0.0;
                for (int j = 0; j < m_matrix_list.Count; j++)
                {
                    a += m_matrix_list[j][i, 0];
                    b += m_matrix_list[j][i, 1];
                    c += m_matrix_list[j][i, 2];
                }
                m_matrix_d[i, 0] = a / m_matrix_list.Count;
                m_matrix_d[i, 1] = b / m_matrix_list.Count;
                m_matrix_d[i, 2] = c / m_matrix_list.Count;
            }
            var l_min = m_matrix_w.Multiply(m_matrix_d).Multiply(m_martix_min);
            var l_max = m_matrix_w.Multiply(m_matrix_d).Multiply(m_martix_max);
            MessageBox.Show(LevelToString(l_min[0,0],l_max[0,0]).ToString());
        }
        private string LevelToString(double min, double max)
        {
            if (min >= -1 && max < -1 / 3)
            {
                return "不安全";
            }
            if (min >= -1 / 3 && max < 1 / 3)
            {
                return "一般安全";
            }
            if (min >= 1 / 3 && max <= 1)
            {
                return "一般安全";
            }
            return "未知安全";
        }
    }
}

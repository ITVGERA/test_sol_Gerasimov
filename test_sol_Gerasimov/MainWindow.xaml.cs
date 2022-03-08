using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using System.Data.Entity;
using test_sol_Gerasimov.EF;
using System.Data.Entity.Migrations;
namespace test_sol_Gerasimov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EFContext context;
        string cmb_selection = "",btn_selection="";
        string habr = "https://habr.com/ru/rss/all/all/";
        string ixbt = "http://www.ixbt.com/export/news.rss";
        int read_sourse, save_sourse, count_pages = 0,cur_page=1;
        List<Cnews> ixbtdata = new List<Cnews>();
        List<Cnews> habrdata = new List<Cnews>();
        List<Cnews> alldata = new List<Cnews>();

        public MainWindow()
        {
            context = new EFContext("Testconn");
            InitializeComponent();
            //context.EFnews.Load();
            //dGrid.DataContext = context.EFnews.Local;
            read_sourse = save_sourse= 0;
        }

        private void InitNewList(string choice,int page)
        {
            dGrid.DataContext = null;
            pagination.Items.Clear(); ;
            int cnt= context.EFnews.Count();
            if (cnt == 0)
            {
                MessageBox.Show("Новостей еще нет в базе");
                return;
            }
            if (String.Compare(choice, "Все") == 0)
            {
                context.EFnews.Load();
                dGrid.DataContext = context.EFnews.Local;
                alldata.AddRange(context.EFnews.Local);
                count_pages = context.EFnews.Count();
                for (int i = 1; i <= (double)count_pages / 10; i++)
                {
                    pagination.Items.Add(i.ToString());
                }

            }
            else
                dGrid.DataContext = context.EFnews.Where(c => c.Sourse == choice).ToList();
            if (String.Compare(btn_selection, "IXBT") == 0)
                ixbtdata.AddRange(context.EFnews.Where(c => c.Sourse == choice).ToList());
            else
                habrdata.AddRange(context.EFnews.Where(c => c.Sourse == choice).ToList());
            count_pages = context.EFnews.Where(c => c.Sourse == choice).Count();
                for (int i = 1; i <= (double)count_pages/10; i++)
                {
                    pagination.Items.Add(i.ToString());
                }
            }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            btn_selection = (string)((MenuItem)e.OriginalSource).Header;
            parse_click(btn_selection);
        }

        private void parse_click(string s)
        {
            switch (s)
            {
                case "IXBT":
                    parse(ixbt);
                    break;
                case "Хабр":
                    parse(habr);
                    break;
                default:
                    MessageBox.Show("Error");
                    break;
            }
        }

        public void parse(string str)
        {
            read_sourse = save_sourse = 0;
            read_news.Content = "Прочитано новостей:";
            save_news.Content = "Сохранено новостей:";
            XmlDocument xDoc = new XmlDocument();
                xDoc.Load(str);
                XmlElement xRoot = xDoc.DocumentElement;
                Cnews obj = new Cnews();
                string s = "";
            if (String.Compare(btn_selection, "Хабр") == 0)
                obj.Sourse = "Хабр";
            else
             if (String.Compare(btn_selection, "IXBT") == 0)
                obj.Sourse = "IXBT";
            //if(String.Compare(str, "Хабр") == 0)

            if (xRoot != null)
                {
                    // обход всех узлов в корневом элементе
                    foreach (XmlElement xnode in xRoot)
                    {


                        // обходим все дочерние узлы элемента 
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (String.Compare(childnode.Name.ToString(), "item") != 0)
                                continue;
                            foreach (XmlNode xmlNode in childnode)
                            {

                                if (String.Compare(xmlNode.Name.ToString(), "title") == 0)
                                {
                                    s += "Title:" + xmlNode.InnerText + "\n";
                                obj.Title = xmlNode.InnerText;
                                  //  MessageBox.Show(s);
                                }
                               else if (String.Compare(xmlNode.Name.ToString(), "guid") == 0)
                                {
                                    s += "Guid:" + xmlNode.InnerText + "\n";
                                obj.Guid = xmlNode.InnerText;
                                //  MessageBox.Show(s);
                            }
                            else if (String.Compare(xmlNode.Name.ToString(), "link") == 0)
                                {
                                    s += "Link:" + xmlNode.InnerText + "\n";
                                obj.Link = xmlNode.InnerText;
                                //  MessageBox.Show(s);
                            }
                            else if (String.Compare(xmlNode.Name.ToString(), "description") == 0)
                                {
                                    s += "Description:" + xmlNode.InnerText + "\n";
                                obj.Description = xmlNode.InnerText;
                                //  MessageBox.Show(s);
                            }
                            else if (String.Compare(xmlNode.Name.ToString(), "pubDate") == 0)

                                {
                                    s += "PubDate:" + xmlNode.InnerText + "\n";
                                DateTime tmp=Convert.ToDateTime(xmlNode.InnerText);
                                obj.PubDate = tmp;
                                // MessageBox.Show(s);
                            }
                        }
                        //MessageBox.Show(s);
                        ++read_sourse;
                        int dbl = context.EFnews.Where(c => c.Guid == obj.Guid).Count();
                        if (dbl == 0)
                        {
                            context.EFnews.Add(obj);
                            context.SaveChanges();
                            ++save_sourse;
                        }
                        
                           
                        s = "";
                        }

                    }
                read_news.Content += read_sourse.ToString();
                save_news.Content += save_sourse.ToString();



            }
           
        }

        private void pagination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cur_page =Convert.ToInt32(pagination.SelectedItem);

        }

        private void sort_date_Click(object sender, RoutedEventArgs e)
        {
            sort_src.IsChecked = false;
            dGrid.DataContext = null;
            if (String.Compare(cmb_selection, "Все") == 0)
                dGrid.DataContext = alldata.OrderBy(c=>c.PubDate);
            else if (String.Compare(cmb_selection, "Хабр") == 0)
                dGrid.DataContext = habrdata.OrderBy(c => c.PubDate);
            else if (String.Compare(cmb_selection, "IXBT") == 0)
                dGrid.DataContext = ixbtdata.OrderBy(c => c.PubDate);
        }

        private void sort_src_Click(object sender, RoutedEventArgs e)
        {
            sort_date.IsChecked = false;
            dGrid.DataContext = null;
            if (String.Compare(cmb_selection, "Все") == 0)
                dGrid.DataContext = alldata.OrderBy(c => c.Sourse);
            else if (String.Compare(cmb_selection, "Хабр") == 0)
                dGrid.DataContext = habrdata.OrderBy(c => c.Sourse);
            else if (String.Compare(cmb_selection, "IXBT") == 0) dGrid.DataContext = ixbtdata.OrderBy(c => c.Sourse);
           

        }

        private void view_Click(object sender, RoutedEventArgs e)
        {
            sort_date.IsChecked = false;
            sort_src.IsChecked = false;
            if (cmb_selection == "")
            {
                MessageBox.Show("Источник новостей не выбран");
                return;
            }

            InitNewList(cmb_selection,cur_page); 
        }

        private void src_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)src_combobox.SelectedItem;
            cmb_selection = ComboItem.Content.ToString();
        }
    }
}

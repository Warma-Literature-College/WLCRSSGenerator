// See https://aka.ms/new-console-template for more information
/*
 思路：收集到一切作者整理好的markdown文档，转换为一段html并转入到模板html存为新文件，然后整理文章目录并生成新的RSS，重命名旧的rss为当前日期，然后指定新的rss为默认名称。
最好能直接推送到服务器对应的网站目录
 
 */
using System;
public class Program
{
    public static void Main()
    {
        Console.WriteLine("沃玛文学院 专属RSS//html生成器");
        Console.WriteLine("A Feiron Iguista Work under Corona.Studio Guiding.");
        Console.WriteLine("请不要瞎折腾 没有什么错误处理。如果一直卡在“输入编码”，请随意输入即可。\n");

        while (true) {
            
            Console.WriteLine("1 转换到网页   2 生成RSS   3 关于   0 退出");

            string ini = Console.ReadLine();

            switch (ini) {
                case "1":

                    Console.WriteLine("改：将目标文件放在./in/下，输入文件名。：");

                    Actions.theHtmlGenerator();
                    break;
                case "2":
                    Console.WriteLine("即将重新生成RSS");

                    Actions.RSSGen();
                    break;

                case "3":
                    Console.WriteLine("作者：Feiron Iguista\n组织：Corona.Studio日冕工作室\n面向：沃玛文学院报务推送系统\n协议：AGPLv3\n");
                    break;
                case "0":
                    Console.WriteLine("即将退出。");
                    return;

                default:
                    Console.WriteLine("?");
                    break;

            }
        }
    }

}
public class Actions
{
    public static void theHtmlGenerator()
    {//生成成品文章的HTML
        
        HTMLG.md2htmlpart();

    }

    public static void RSSGen()
    {
        rssgen.genere();

    }
}


public class HTMLG
{
    string[] mdwn = { };//md导入为字符串数组，然后替换语法记号，然后存入导出数组，然后导出。

    public static async Task md2htmlpart()
    {//转换md到html 替换语法记号
        while (true)
        {

            string intext;
            string auffcont;
            string? inputpath = Console.In.ReadLine();
            if (inputpath == null) { break; } else//???????
            {
                string inputpathe = "./in/" + inputpath;
                while (true) { 
   
                    Console.WriteLine("输入编码");
                    string? ili = Console.ReadLine();

                    if(ili == null) { break; } else
                    {
                        if (ili.Equals("utf8") || ili.Equals("UTF8") || ili.Equals("UTF-8") || ili.Equals("utf-8"))
                        {
                            intext = System.IO.File.ReadAllText(@inputpathe);
                            ngpp(intext);
                            break;

                        }
                        else if (ili.Equals("ansi") || ili.Equals("ANSI"))
                        {
                            intext = System.IO.File.ReadAllText(@inputpathe, System.Text.Encoding.UTF8);
                            ngpp(intext);
                            break;

                        }
                        else
                        {
                            Console.WriteLine("??");
                            continue;
                        }
                    }
                }
                break;
            }
        }
    }

    public static async Task ngpp(String intext)
    {
        string auffcont = CommonMark.CommonMarkConverter.Convert(intext);
        //System.IO.File.ReadAllText(intext).Dispose();
        string? passageName = "default";
        string? authorr = passageName;
        Console.WriteLine("输入文件名");
        passageName = Console.ReadLine();
        Console.WriteLine("输入作者名");
        authorr = Console.ReadLine();
        string auffcont0 = auffcont.Replace("<p>", "<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        await kombiner(passageName,authorr , auffcont0);

    }


    public static async Task kombiner(string passageName,string authr,string mdwn0)
    {
        string defaultlocal = "./post/";
        System.DateTime sysTime = System.DateTime.Now;
        string stime = sysTime.ToString();

        string[] lines =
        {
            "<!DOCTYPE html><head><meta charset=\"utf-8\"><style type=\"text/css\">@import url(./../../srca/s.css);</style><title>"+passageName+"</title><link rel=\"shortcut icon\" href = \"./../../srca/i/favicon.png\" ><link rel = \"Bookmark\" href = \"./../../srca/i/favicon.png\" ></ head ><body class=\"ci\"><div class=\"in\">",
            "<br><h3 style=\"text-align:center\">",passageName,"</h3>",
            "<h6 style=\"text-align:center\">作者：",authr,"</h6>",

            mdwn0,
            "</div></body><footer><p id=\"isd\">文章生成于",stime,"，页面详情参见<a href=\"https://wlc.vjemja.su\">主页</a>的页脚</p><br><br><br></footer>"
        };

        System.IO.Directory.CreateDirectory(@defaultlocal);

        Console.WriteLine("导出已完成。");

        await File.WriteAllLinesAsync(@defaultlocal+passageName+"+"+authr + ".html", lines);
        return;
    }
}

public class rssgen
{
    public static async Task genere()
    {//foreach读取/post目录下的所有html然后存到string数组，然后foreach writeall
        
        DirectoryInfo di = new DirectoryInfo(@"./post/");
        FileInfo[] dirs = di.GetFiles();
        int counts = dirs.Length;
        string[] rssin = new string[counts];

        for(int i = 0; i< dirs.Length; i++)
        {            
            rssin[i] = dirs[i].Name;
        }
        int arc = 0;

        foreach (string fn in rssin)
        {
            FileInfo fl = new FileInfo("./post/"+fn);
            string pdate = fl.CreationTime.ToString("yyy-mm-dd HH:mm:ss");
            string passage = fn;
            string[] arac = passage.Split('+','.');
            string authori=arac[1];
            string passageo= arac[0];

            rssin[arc] = ("\n<item><title>"+passageo+"</title><link>https://wlc.vjemja.su/cate/post/"+fn+ "</link><description>作者："+authori+ "</description><pubDate>"+pdate+"</pubDate><category>" + authori+"</category></item>\n");

            arc++;
        }

        string rssend = "</channel></rss>";
        string rssstart = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><rss version=\"2.0\"><channel><title>沃玛文学院报务系统</title><link>https://wlc.vjemja.su</link><description> Warma Literature Collage PapersPush </description>";

        string generatio = String.Join("\n", rssin);

        string[] lines =
        {
            
            rssstart,generatio,rssend,
           
        };
        System.IO.File.Delete(@"./rss.xml");
        Console.WriteLine("完成");
        await File.WriteAllLinesAsync("rss.xml", lines);

    }
}
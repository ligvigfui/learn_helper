using System;
using Newtonsoft.Json;
using pszeudo_random_base;
using System.Collections.Specialized;



namespace Extensions
{
    public static class Extensions
    {
        public static int custom_probability(this Trait trait)
        {
            return (int)trait.linear_probability + 3;
        }
    }
}
namespace Program
{
    using Extensions;
    
    interface Iname{
        string name {get; set;}
    }
    interface Idisplayable : Iname{
        List<object>? properties {get; set;}
        Action? called {get; set;} 
        Classes? classes {get; set;}
    }
    interface Idisplayable<T> : Iname{
        List<object>? properties {get; set;}
        Action<T>? called {get; set;}
    }
    /// <summary>
    /// .properties[0] = ConsoleColor forgroud
    /// <br>.properties[1] = ConsoleColor backgroud </br>
    /// </summary>
    class ConsoleColorExtension : Displayable{
        public ConsoleColorExtension(string name, ConsoleColor foreground, ConsoleColor background) : base(name)
        {
            this.properties = new List<object>(){foreground, background};
        }
    }
    class Displayable : Idisplayable{
        public string name {get; set;}
        // can conrtain other displayables or list of strings
        public List<object>? properties {get; set;}
        public Action? called {get; set;}
        public Classes? classes {get; set;}
        public void Add(object property){
            if (called == null){
                throw new Exception("Displayable must have a called function before adding properties");
            }
            if(properties == null){
                properties = new List<object>();
            }
            properties.Add(property);
        }
        public void AddRange(List<object> properties){
            if (called == null){
                throw new Exception("Displayable must have a called function before adding properties");
            }
            if(properties == null){
                properties = new List<object>();
            }
            properties.AddRange(properties);
        }
        public void AddAction(Action action){
            called = action;
        }
        public static void chose_from_list(List<object> list)
        {
            
        }
        public Displayable(string name, Action? called = null, Classes? classes = null, List<object>? properties = null){
            this.name = name;
            this.called = called;
            this.classes = classes;
            this.properties = properties;
        }
        public Displayable(string name){
            this.name = name;
        }
        public Displayable(string name, Action called){
            this.name = name;
            this.called = () => called();
        }
    }
    class Classes : Displayable
    {
        /// <summary>
        /// key: name as string, value: shortened name as string
        /// <br>shortened name should be unique and not longer than 3 characters</br>
        /// </summary>
        public new OrderedDictionary? properties;
        public ConsoleColorExtension? color;
        public static Dictionary<string, Classes> classes_list = new Dictionary<string, Classes>();
        /// <summary>
        /// key: name as string, value: shortened name as string
        /// <br>shortened name should be unique and not longer than 3 characters</br>
        /// </summary>
        public void Add(object name, object shorted_name)
        {
            if (properties == null)
            {
                properties  = new OrderedDictionary();
            }
            properties.Add(name, shorted_name);
        }
        /// <summary>
        /// key: name as string, value: shorted name as string
        /// <br>shortened name should be unique and not longer than 3 characters</br>
        /// </summary>
        public void AddRange(List<pairs> pairs)
        {
            if (properties == null)
            {
                properties  = new OrderedDictionary();
            }
            foreach (var pair in pairs)
            {
                properties.Add(pair.object1, pair.object2);
            }
        }
        public List<string>? get_names()
        {
            if (properties == null)
            {
                return null;
            }
            List<string> names = new List<string>();
            names = properties!.Keys.Cast<string>().ToList();
            for (int i = 0; i < names.Count; i++){
                if (properties[names[i]] != null){
                    names[i] = ((string)properties[names[i]]!).PadRight(3) + ": ";
                } else {
                    names[i] = names[i].PadRight(3) + ": ";
                }
            }
            return names;
        }
        public Classes(string name, OrderedDictionary? properties = null, ConsoleColorExtension? color = null) : base(name)
        {
            this.properties = properties;
            this.color = color;
            classes_list.Add(name, this);
        }
        public Classes(string name) : base(name)
        {
            classes_list.Add(name, this);
        }
    }
    class pairs{
        public object object1;
        public object object2;
        public pairs(object object1, object object2)
        {
            this.object1 = object1;
            this.object2 = object2;
        }
    }
    class Program
    {
        static int line = 1, selection_start;
        static int setting_rows, setting_margo;
        static string header = "";
        static ConsoleKeyInfo keyInfo;
        static Random random = new Random();
        static Stack<Idisplayable> menu_call_stack = new Stack<Idisplayable>();
        static Stack<int> line_call_stack = new Stack<int>();
        // this starting value will go out of scope before the first call of the menu
        public static ConsoleColorExtension selected_color = new ConsoleColorExtension("selected", ConsoleColor.Black, ConsoleColor.White);
        public static void Write_list(List<object> list){
            foreach (var item in list)
            { 
                Write(item);
            }
        }
        public static void SetColor(ConsoleColorExtension color){
            Console.ForegroundColor = ((ConsoleColor)color.properties![0]);
            Console.BackgroundColor = ((ConsoleColor)color.properties![1]);
        }
        public static void Write(params object[] oo)
        {
            foreach (var o in oo)
                if (o is ConsoleColorExtension) selected_color = (ConsoleColorExtension)o;
                else Console.Write(o.ToString());
        }
        public static void clear_and_reset_color()
        {
            Console.Write("a");
            Console.Clear();
            Console.ResetColor();
        }
        private static string get_header(Displayable menu )
        {
            string header = "";
            foreach (var item in menu_call_stack)
            {
                header += item.name + " > ";
            }
            header += menu.name + " >";
            if (header.Length > setting_margo)
            {
                header = "..." + header.Substring(header.Length - setting_margo+3);
            }
            return header;
        }
        static void Sleep()
        {
            Thread.Sleep(random.Next(80, 100));
        }
        private static void display_menu(Displayable menu)
        {
            if (menu.properties == null) throw new Exception("You souldn't be here > display_menu > menu.properties == null");
            header = get_header(menu);
            line = 0;
            selection_start = 0;
            bool Continue = true;
            object[] menu_display = new object[setting_rows > menu.properties!.Count+1 ? menu.properties!.Count+1 : setting_rows];
            clear_and_reset_color();
            do
            {
                if (setting_rows > menu.properties!.Count+1)
                {
                    menu_display[0] = header;
                    for (int i = 1; i < menu.properties.Count+1; i++)
                    {
                        menu_display[i] = menu.properties[i-1];
                    }
                }
                //selection bounds
                if (line < selection_start)
                {
                    selection_start = line;
                    menu_display[0] = header;
                    for (int i = selection_start + 1; i < setting_rows + selection_start; i++)
                    {
                        menu_display[i + 1] = menu.properties![i];
                    }
                }
                if (line > selection_start + setting_rows)
                {
                    selection_start = line - setting_rows;
                    menu_display[0] = header;
                    for (int i = selection_start + 1; i < setting_rows + selection_start; i++)
                    {
                        menu_display[i + 1] = menu.properties![i];
                    }
                }
                //display
                Console.ResetColor();
                Console.WriteLine(menu_display[0]);
                for (int i = 1; i < menu_display.Count() ; i++)
                {
                    if (menu_display[i] is null) throw new NullReferenceException("menu_display[i] is null");
                    if (i == line - selection_start)
                    {
                        SetColor(selected_color);
                    } else 
                    if (((Displayable)menu_display[i]).classes is not null) { 
                        if (((Displayable)menu_display[i]).classes!.color != null)
                        {
                            SetColor(((Displayable)menu_display[i]).classes!.color!);
                        }
                    }
                    if (((Displayable)menu_display[i]).name.Length > setting_margo-3)
                        Console.Write(((Iname)menu_display[i]).name.Substring(0, setting_margo-3) + "...\n");
                    else
                        Console.Write(((Iname)menu_display[i]).name.PadRight(setting_margo) + "\n");
                }
                Console.ResetColor();
                //controlling
                keyInfo = Console.ReadKey(true);
                Thread.Sleep(random.Next(80, 100));
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (line <= 1)
                    {
                        line = menu.properties!.Count - 1;
                    }
                    else
                    {
                        line--;
                    }
                }
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (line >= menu.properties!.Count - 1)
                    {
                        line = 1;
                    }
                    else
                    {
                        line++;
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // if class is menu then the default action is to display the menu
                    // if class not menu then the default action is to call the edit or add function
                    if (menu.properties![line] is not Displayable)
                    {
                        Edit_or_Add_more_data(menu.properties![line]);
                    }
                    if (((Displayable)menu.properties![line]).called != null)
                    {
                        menu_call_stack.Push(menu);
                        line_call_stack.Push(line);
                        ((Displayable)menu.properties![line]).called!();
                        menu = (Displayable)menu_call_stack.Pop();
                        header = get_header(menu);
                        line = line_call_stack.Pop();
                    }
                    if (((Displayable)menu.properties![line]).called == null)
                    {
                        menu_call_stack.Push(menu);
                        line_call_stack.Push(line);
                        display_menu((Displayable)menu.properties[line]);
                        menu = (Displayable)menu_call_stack.Pop();
                        header = get_header(menu);
                        line = line_call_stack.Pop();
                    }
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Continue = false;
                }
            } while (Continue);
        }
        public static void Edit_or_Add_more_data(object data)
        { 
            //edit or add more data
        }
        public static void setup_initial_manus(Displayable menu, Classes _class)
        {
            foreach (var item in menu.properties!)
            {
                if (item is Displayable)
                {
                    ((Displayable)item).classes = _class;
                    if (((Displayable)item).properties != null)
                    {
                        setup_initial_manus((Displayable)item, _class);
                    }
                }

            }

        }
        static void Main(string[] args)
        {
            setting_margo = Console.WindowWidth/2 > 20 ? Console.WindowWidth/2 : 20;
            setting_rows = Console.WindowHeight - 1 > 10? Console.WindowHeight - 1 : 10;
            
            
            Displayable menu = new Displayable("Main menu", properties: new List<object>{
                new Displayable("Raw data"),
                new Displayable("Settings", properties: new List<object>{
                    new Displayable("Colors", properties: new List<object>{
                        new ConsoleColorExtension("Menu", ConsoleColor.White, ConsoleColor.Black),
                        new ConsoleColorExtension("Selected", ConsoleColor.Black, ConsoleColor.White),
                        new ConsoleColorExtension("Info", ConsoleColor.DarkGray, ConsoleColor.Black),
                        new ConsoleColorExtension("Container", ConsoleColor.Cyan, ConsoleColor.Black),
                        new ConsoleColorExtension("Data", ConsoleColor.Green, ConsoleColor.Black)
                    }),
                    new Displayable("Classes", properties: new List<object>{
                        new Classes("Menu"),
                        new Classes("Color", properties: new OrderedDictionary{
                            {"foreground", null},
                            {"background", null}
                        })
                    }),
                    new Displayable("Rows"),
                }),
                new Displayable("About", properties: new List<object>{
                    new Displayable("a")
                }),
                new Displayable("Exit")
            });
            //main menu > settings > classes > menu.color = menu > classes > color > menu
            ((Classes)((Displayable)((Displayable)menu.properties![1]).properties![1]).properties![0]).color =
            ((Displayable)((Displayable)menu.properties![1]).properties![0]).properties![0] as ConsoleColorExtension;
            //setup the menu fields for each menu
            setup_initial_manus(menu, ((Classes)((Displayable)((Displayable)menu.properties![1]).properties![1]).properties![0]));
            bool Continue = true;
            selected_color = ((ConsoleColorExtension)((Displayable)((Displayable)menu.properties![1]).properties![0]).properties![1]);
            Console.WriteLine("Welcome to Ligvigfui's GYTK learning software! v1.0.0");
            do {
                display_menu(menu);
                clear_and_reset_color();
                Console.WriteLine("Do you want to exit? (y/n)");
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    Continue = false;
                }
            } while (Continue);
            // save?

        }

    }
        // 12-13 perc, 22-23 perc, 37-38, 76-77
}
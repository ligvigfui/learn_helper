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
            colors.Add(name, this);
        }
        public static Dictionary<string, ConsoleColorExtension> colors = new Dictionary<string, ConsoleColorExtension>();
        // this sould be done in Program main()
        private static List<ConsoleColorExtension> color_list = new List<ConsoleColorExtension> { new ConsoleColorExtension("data", ConsoleColor.Green , ConsoleColor.Black),
            new ConsoleColorExtension("selected", ConsoleColor.Black, ConsoleColor.White), new ConsoleColorExtension("container", ConsoleColor.Cyan, ConsoleColor.Black),
            new ConsoleColorExtension("info", ConsoleColor.DarkGray, ConsoleColor.Black), new ConsoleColorExtension("menu", ConsoleColor.White, ConsoleColor.Black)};
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
        }
        public Classes(string name) : base(name)
        {
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
        static int line, selection_start;
        static int setting_rows, setting_margo;
        static string header = "";
        static ConsoleKeyInfo keyInfo;
        static Random random = new Random();
        static Stack<Idisplayable> menu_call_stack = new Stack<Idisplayable>();
        static Stack<int> line_call_stack = new Stack<int>();
        public static void Write_list(List<object> list){
            foreach (var item in list)
            { 
                Write(item);
            }
        }
        public static void Write(params object[] oo)
        {
            foreach (var o in oo)
            {
                if (o is ConsoleColorExtension)
                {
                    Console.ForegroundColor = ((ConsoleColor)((ConsoleColorExtension)o).properties![0]);
                    Console.BackgroundColor = ((ConsoleColor)((ConsoleColorExtension)o).properties![1]);
                }
                else Console.Write(o.ToString());
            }
        }
        public static void clear_and_reset_color()
        {
            Console.Write("a");
            Console.Clear();
            Console.ResetColor();
        }
        private static string get_header()
        {
            string header = "";
            foreach (var item in menu_call_stack)
            {
                header += item.name + " > ";
            }
            if (header.Length > setting_margo)
            {
                header = "..." + header.Substring(header.Length - setting_margo-3);
            }
            return header;
        }
        private static void display_menu(Displayable menu)
        {
            header = get_header();
            line = 0;
            selection_start = 0;
            bool Continue = true;
            object[] menu_display = new object[setting_rows];
            clear_and_reset_color();
            if (menu.properties == null)
            {
                throw new Exception("You souldn't be here");
            }
            do
            {
                if (setting_rows > menu.properties!.Count+1)
                {
                    menu_display[0] = header;
                    for (int i = 1; i < menu.properties.Count; i++)
                    {
                        menu_display[i] = menu.properties[i];
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
                for (int i = 0; i < setting_rows; i++)
                {
                    if (i == line - selection_start)
                    {
                        Write(ConsoleColorExtension.colors["selected"]);
                    }
                    // if (((Displayable)menu_display[i+selection_start]).classes is )
                    //make this function display containers
                    Write(((Iname)menu_display[i]).name, ConsoleColorExtension.colors["menu"], "\n");
                }
                //controlling
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (line <= 0)
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
                        line = 0;
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
                        header = get_header();
                        line = line_call_stack.Pop();
                    }
                    if (((Displayable)menu.properties![line]).called == null)
                    {
                        menu_call_stack.Push(menu);
                        line_call_stack.Push(line);
                        display_menu((Displayable)menu.properties[line]);
                        menu = (Displayable)menu_call_stack.Pop();
                        header = get_header();
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
        public static void setup_initial_manu_colors(Displayable menu, Classes classes)
        {
            foreach (var item in menu.properties!)
            {
                if (item is Displayable)
                {
                    ((Displayable)item).classes = classes;
                }
            }
        }
        static void Main(string[] args)
        {
            setting_margo = Console.WindowWidth/2;
            setting_rows = Console.WindowHeight - 1;
            
            
            Displayable menu = new Displayable("Main menu", properties: new List<object>{
                new Displayable("Raw data", properties: new List<object>{
                    new Displayable("")
                }),
                new Displayable("Settings", properties: new List<object>{
                    new Displayable("Colors", properties: new List<object>{
                        
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
                    new Displayable("")
                }),
                new Displayable("Exit")
            });
            //main menu > settings > classes > menu.color = menu > classes > color > menu
            //((Displayable)menu.properties![1]).properties
            bool Continue = true;
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
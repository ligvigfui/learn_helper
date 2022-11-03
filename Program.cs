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
    class ConsoleColorExtension : Iname{
        public ConsoleColorExtension( string name, ConsoleColor foreground, ConsoleColor background){
            this.name = name;
            this.foreground = foreground;
            this.background = background;
            colors.Add(this.name, this);
        }
        public string name {get; set;}
        public ConsoleColor foreground;
        public ConsoleColor background;
        public static Dictionary<string, ConsoleColorExtension> colors = new Dictionary<string, ConsoleColorExtension>();
        private static List<ConsoleColorExtension> color_list = new List<ConsoleColorExtension> { new ConsoleColorExtension("data", ConsoleColor.Green , ConsoleColor.Black),
            new ConsoleColorExtension("selected", ConsoleColor.Black, ConsoleColor.White), new ConsoleColorExtension("container", ConsoleColor.Cyan, ConsoleColor.Black),
            new ConsoleColorExtension("info", ConsoleColor.DarkGray, ConsoleColor.Black), new ConsoleColorExtension("default", ConsoleColor.White, ConsoleColor.Black)};
    }
    class MenuDisplayable : Idisplayable{
        public string name {get; set;}
        // can conrtain other displayables or list of strings
        public List<object>? properties {get; set;}
        public Action? called {get; set;}
        public Classes? classes {get; set;}
        public void Add(object property){
            if (called == null){
                throw new Exception("MenuDisplayable must have a called function before adding properties");
            }
            if(properties == null){
                properties = new List<object>();
            }
            properties.Add(property);
        }
        public void AddRange(List<object> properties){
            if (called == null){
                throw new Exception("MenuDisplayable must have a called function before adding properties");
            }
            if(properties == null){
                properties = new List<object>();
            }
            properties.AddRange(properties);
        }
        public MenuDisplayable(string name, Action? called = null, List<object>? properties = null){
            this.name = name;
            this.called = called;
            this.properties = properties;
        }
        public MenuDisplayable(string name){
            this.name = name;
        }
        public MenuDisplayable(string name, Action called){
            this.name = name;
            this.called = () => called();
        }
    }
    class Program
    {
        static int line, selection_start;
        static int setting_rows;
        static string header = "";
        static ConsoleKeyInfo keyInfo;
        static Random random = new Random();
        static Stack<Idisplayable> menu_call_stack = new Stack<Idisplayable>();
        static Stack<string> name_call_stack = new Stack<string>();
        static Stack<int> line_call_stack = new Stack<int>();
        public static void Write(params object[] oo)
        {
            foreach (var o in oo)
                if (o is ConsoleColorExtension) { Console.ForegroundColor = ((ConsoleColorExtension)o).foreground; Console.BackgroundColor = ((ConsoleColorExtension)o).background; }
                else Console.Write(o.ToString());
        }
        public static void clear_and_reset_color()
        {
            Console.Write("a");
            Console.Clear();
            Console.ResetColor();
        }
        /*
        private static void no_action_up(List<Idisplayable> menus)
        {
            if (menus[line].called == null)
            {
                line--;
                no_action_up(menus);
            }
        }
        private static void no_action_down(List<Idisplayable> menus)
        {
            if (menus[line].called == null)
            {
                line++;
                no_action_down(menus);
            }
        }*/
        public static void display_menu(MenuDisplayable menu)
        {
            if (menu.properties == null)
            {
                throw new Exception("MenuDisplayable must have properties before displaying");
            }
            else
            {
                display_menu_for_the_3rd_time_ffs(menu);
            }
        }
        private static void display_menu_for_the_3rd_time_ffs(MenuDisplayable menu)
        {
            header = header + "> " + menu.name;
            line = 0;
            selection_start = 0;
            bool Continue = true;
            object[] menu_display = new object[setting_rows];
            clear_and_reset_color();
            do
            {
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
                for (int i = selection_start; i < setting_rows + selection_start; i++)
                {
                    if (i == line)
                    {
                        Write(ConsoleColorExtension.colors["selected"], ((Iname)menu_display[i]).name, ConsoleColorExtension.colors["default"]);
                    }
                    else
                    {
                        //make this function display containers
                        Write(((Iname)menu_display[i]).name);
                    }
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
                    if (menu.properties![line] is MenuDisplayable)
                    {
                        menu_call_stack.Push(menu);
                        name_call_stack.Push(header);
                        line_call_stack.Push(line);
                        display_menu_for_the_3rd_time_ffs((MenuDisplayable)menu.properties[line]);
                        menu = (MenuDisplayable)menu_call_stack.Pop();
                        header = name_call_stack.Pop();
                        line = line_call_stack.Pop();
                    }
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Continue = false;
                }
            } while (Continue);


        }
    }
    /// <summary>
    /// NOT FINAL JET
    /// </summary>
    class DataDisplayable : Data_logic, Idisplayable
    {
        // solve case when multiple subjects have the same data. How can I have different values for the same data.traits?
        public string name { get; set; }
        public Classes classes;
        public new List<object>? properties { get; set; }
        public Action called { get; set; }

        public DataDisplayable clone()
        {
            return new DataDisplayable(this.name, this.classes, this.called, this.properties);
        }
        public void upgrade()
        {

        }
        public DataDisplayable(string name, Classes classes, Action called, List<object>? properties = null)
        {
            this.name = name;
            this.classes = classes;
            this.called = called;
            this.properties = properties;
        }
    }
    // OLD CLASSES BELOW
    class Classes : Idisplayable
    {
        public string name { get; set; }
        public List<object>? properties { get; set; }
        public Action? called { get; set; }
        public Classes? classes { get; set; }
        
        public OrderedDictionary? Class_properties { get; set; }
        //if Action is null, then the class has a custom action
        public Classes(string name, Action? called = null, List<object>? properties = null)
        {
            this.name = name;
            this.called = called;
            this.properties = properties;
        }
        public Classes menu = new Classes("menu", properties: new List<object>{"menu"});
    }
    class Menu
    {
        public enum menu_enter_type
        {
            custom,
            same_for_all
        }
        public byte first_some_only_to_show = 1;
        //the first some items in the object list are shown but you sould not be able to highlight them they also have empty actions
        public List<object> menu_list = new List<object>();
        public List<Action> menu_enter = new List<Action>();
        public bool custom = false;
        public void AddRange(List<object> to_display, List<Action> to_do)
        {
            if (this.menu_list.Any() && !this.menu_enter.Any()) throw new ArgumentException("This is an info list. Use AddInfoRange to expand it");
            for (int i = 0; i < to_display.Count; i++)
            {
                this.menu_enter.Add(to_do[i]);
                this.menu_list.Add(to_display[i]);
            }
        }
        public void AddInfoRange(List<object> to_display)
        {
            if (this.menu_enter.Any()) throw new ArgumentException("This is an interactive list. Use AddRange to expand it");
            for (int i = 0; i < to_display.Count; i++)
            {
                this.menu_list.Add(to_display[i]);
            }
        }
    }

    class main
    {
        #region 
        static List<List<object>> raw_data = new List<List<object>>();
        static Stack<int> line_call_stack = new Stack<int>();
        static Stack<object> name_call_stack = new Stack<object>();
        static ConsoleKeyInfo keyInfo;
        static int line = 0, last_line;
        static string? header;
        static bool runing = true;
        static Random random = new Random();
        static int setting_rows = 10, setting_margo;
        #endregion
        //params: 0 = setting_margo
        public static void Main()
        {
            setting_margo = Console.WindowWidth / 2;
            Data_logic.init_debug_file();
            Trait default_trait = new Trait(2); default_trait.probability = () => default_trait.custom_probability();
            //set up menus
            #region 

            Menu MainMenu = new Menu();
            Menu Options = new Menu();
            Menu Raw_Data = new Menu();
            Menu tips_and_tricks = new Menu();
            MainMenu.AddRange(
                new List<object> { "Main menu", "Subjects", "Raw data", "Tips and triks", "Options", "Exit" },
                new List<Action> { () => { }, () => { }, () => UIMenuDisplay(Raw_Data), () => UIMenuDisplay(tips_and_tricks), () => UIMenuDisplay(Options), () => { } }
            );
            Options.first_some_only_to_show = 2;
            Options.AddRange(
                new List<object> { "Options", new List<object> { ConsoleColorExtension.colors["info"], "Press ESC to go back to menu" }, "Colors", "Numer of lines", "Margos" },
                new List<Action> { () => { }, () => { }, () => { }, () => { }, () => { setting_margo = Convert.ToInt32(enter_data(new List<object> { "Where would you liket to have the margos placed? Current: \n", $"{"stuff",-20} | hihi" })); } }
            );
            Raw_Data.AddRange(
                new List<object> { "Raw Data", "hihi", "3", "4", "5", "6", "7", "8", "9", "3", "4", "5", "6", "7", "8", "9" },
                new List<Action> { () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { }, () => { } }
            );
            tips_and_tricks.AddInfoRange(
                new List<object> { "Tips and tricks", "hihi" }
            );

            #endregion
            //read in the options 

            /*read in 
            - traits list
            - contained drug list
            - subject list
            */
            //main loop
            Console.Clear();

            Write("Welcome to Ligvigfui's GYTK learning software! v1.0\n");
            do
            {
                header = "";
                UIMenuDisplay(MainMenu);
                clear_and_reset_color();
                Console.WriteLine("Are you sure you want to exit?\nYes\tNo");
                Thread.Sleep(random.Next(80, 100));
                keyInfo = Console.ReadKey();
                clear_and_reset_color();
                if (keyInfo.Key == ConsoleKey.Y || keyInfo.Key == ConsoleKey.Escape) runing = false;
            } while (runing);
            //save everithing
        }
        static void clear_and_reset_color()
        {
            Console.WriteLine("A"); Console.ResetColor(); Console.Clear();
        }
        // raw data -> add data of some kind -> update data list -> update display -> 
        // create raw data -> enter in list -> container / data_logic -> enter name -> add to listList -> 
        static void UIDataDisplay(Menu menu)
        {
            //display 2 rows side by side
        }

        static void UIMenuDisplay(Menu menu)
        {
            if (menu.menu_enter.Any())
            {
                UIMenuDisplayOptions(menu);
            }
            else UIMenuDisplayInfo(menu);
        }
        static void UIMenuDisplayOptions(Menu menu)
        {
            /*display frozen rows DONE:
                display call stack 
                display setting_rows-frozen number of rows
                indent data correctly
                display options of the data in right
            */
            int start, end;
            Write(ConsoleColorExtension.colors["default"]);
            line = menu.first_some_only_to_show;
            header = header + "> " + menu.menu_list[0] + " ";
            bool Continue = true;
            do
            {
                start = line - setting_rows / 2 + menu.first_some_only_to_show;
                end = setting_rows + start - menu.first_some_only_to_show;
                if (menu.menu_list.Count < setting_rows) { start = menu.first_some_only_to_show; end = menu.menu_list.Count; } //kisebb lista mint display
                if (start < menu.first_some_only_to_show) { start = menu.first_some_only_to_show; end = setting_rows; }
                if (end > menu.menu_list.Count) { end = menu.menu_list.Count; start = end - setting_rows + menu.first_some_only_to_show; }
                Console.WriteLine(header);
                for (int i = 1; i < menu.first_some_only_to_show; i++)
                {
                    write_from_list(menu, i);
                }
                for (int i = start; i < line; i++)
                {
                    write_from_list(menu, i);
                }
                Write("selected", menu.menu_list[line], "\n", ConsoleColorExtension.colors["default"]);
                for (int i = line + 1; i < end; i++)
                {
                    write_from_list(menu, i);
                }
                Thread.Sleep(random.Next(90, 100));
                keyInfo = Console.ReadKey();
                Console.Clear();
                if (keyInfo.Key == ConsoleKey.DownArrow) { line = line == menu.menu_enter.Count - 1 ? menu.first_some_only_to_show : line + 1; }
                else if (keyInfo.Key == ConsoleKey.UpArrow) { line = line == menu.first_some_only_to_show ? menu.menu_enter.Count - 1 : line - 1; }
                else if (keyInfo.Key == ConsoleKey.RightArrow) { }
                else if (keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Spacebar)
                {
                    line_call_stack.Push(line); name_call_stack.Push(header);
                    menu.menu_enter[(int)line]();
                    line = line_call_stack.Pop(); header = name_call_stack.Pop().ToString()!;
                }
                else if (keyInfo.Key == ConsoleKey.Escape) { Continue = false; }
            } while (Continue);
            clear_and_reset_color();
        }
        static void UIMenuDisplayInfo(Menu menu)
        {
            Write(ConsoleColorExtension.colors["default"]);
            Console.WriteLine(header + "> " + menu.menu_list[0]);
            for (int i = 1; i < menu.menu_list.Count; i++)
            {
                write_from_list(menu, i);
            }
            keyInfo = Console.ReadKey();
            clear_and_reset_color();
        }
        static string? enter_data(List<object> question)
        {
            Console.ResetColor();
            Console.WriteLine();
            foreach (object o in question)
            {
                Write(o);
            }
            Console.WriteLine();
            return Console.ReadLine();
        }
        static string? enter_data(string question)
        {
            Console.ResetColor();
            Console.WriteLine("\n" + question + "\n");
            return Console.ReadLine();
        }
        static void write_from_list(Menu menu, int i)
        {
            if (menu.menu_list[i] is pszeudo_random_base.DLContainer) Write(ConsoleColorExtension.colors["container"], ((Iname)((DLContainer)menu.menu_list[i]).list_data_Logic[0]).name, "\n", ConsoleColorExtension.colors["default"]);
            else if (menu.menu_list[i] is Iname) Write(ConsoleColorExtension.colors["data"], ((Iname)menu.menu_list[i]).name, "\n", ConsoleColorExtension.colors["default"]);
            else if (menu.menu_list[i] is List<object>) { foreach (object o in (List<object>)menu.menu_list[i]) { Write(o); }; Write(ConsoleColorExtension.colors["default"], "\n"); }
            else Write(ConsoleColorExtension.colors["default"], menu.menu_list[i], "\n");
        }
        static void change_row()
        {
            Console.Clear();
            Write(ConsoleColorExtension.colors["default"], "How many rows would you like on the screen at once? Current is: ", setting_rows);
            Write(ConsoleColor.Gray, "5 < x < 4294967295", ConsoleColorExtension.colors["default"]);
            try
            {
                uint a = Convert.ToUInt32(Console.ReadLine());
                if (a < 6)
                {
                    Write("5 is the minimum amount accepted");
                    Console.ReadKey();
                    change_row();
                }
            }
            catch (System.Exception)
            {
                Write("NaN, fuck you!");
                Console.ReadKey();
                change_row();
            }
            //TODO: exit somehow
        }
        static void change_color()
        {

        }
        public static void Write(params object[] oo)
        {
            foreach (var o in oo)
                if (o is ConsoleColorExtension) { Console.ForegroundColor = ((ConsoleColorExtension)o).foreground; Console.BackgroundColor = ((ConsoleColorExtension)o).background; }
                else Console.Write(o.ToString());
        }
    }
        //color parts of the console
        // 12-13 perc, 22-23 perc, 37-38, 76-77

        /*
        TODO:
        !!!!github repo setup
        !make data types configurable runtime instead of compile time
        !!add data
        !veiw data
            sort data alphabeticaly
        -functioning menu
        !save and load data
            save when modiffied
            load at start
            - unload data
        --generate questions automaticly
        --- same data different weight for data_logic for different subjects
        ---- always on top, limit time you can use your PC
        ---- sounds for the controlls
        ---- multiple languange packs
        ---- 


        */
}
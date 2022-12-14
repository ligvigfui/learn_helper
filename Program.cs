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
        public static Dictionary<string, ConsoleColorExtension> colors = new Dictionary<string, ConsoleColorExtension>();
        public ConsoleColorExtension(string name, ConsoleColor foreground, ConsoleColor background) : base(name)
        {
            this.properties = new List<object>(){foreground, background};
            colors.Add(name, this);
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
        public bool opens_in_line = false;
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
    //FIXME: not jet implemented
    class Controll : Displayable{
        //properties contain one controll's key's
        //console key converted to string with Modifiers
        //if key == something in dictionary then do the action corresponding to that key
        public static Dictionary<string, Controll> controlls = new Dictionary<string, Controll>();
        public Controll(string name) : base(name)
        {
            controlls.Add(name, this);
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
                if (o is ConsoleColorExtension) SetColor((ConsoleColorExtension)o);
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
            line = 1;
            selection_start = 0;
            bool Continue = true;
            object[] menu_display;
            clear_and_reset_color();
            do
            {
                if (setting_rows > menu.properties!.Count+1)
                {
                    menu_display = new object[menu.properties!.Count+1];
                    menu_display[0] = header;
                    for (int i = 1; i < menu.properties.Count+1; i++)
                    {
                        menu_display[i] = menu.properties[i-1];
                    }
                } else menu_display = new object[setting_rows];
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
                clear_and_reset_color();
                Console.WriteLine(menu_display[0]);
                for (int i = 1; i < menu_display.Count() ; i++)
                {
                    if (menu_display[i] is null) throw new NullReferenceException("menu_display[i] is null");
                    if (i == line - selection_start)
                    {
                        SetColor(ConsoleColorExtension.colors["Selected"]);
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
                        line = menu.properties!.Count;
                    }
                    else
                    {
                        line--;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (line >= menu.properties!.Count)
                    {
                        line = 1;
                    }
                    else
                    {
                        line++;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow){
                    //TODO: LINE IS 99%> PROPERTIES COUNT
                    if (menu.properties![line] is not Displayable) continue;
                    menu_call_stack.Push(menu);
                    line_call_stack.Push(line);
                    if (((Displayable)menu.properties![line-1]).called != null)
                    {
                        ((Displayable)menu.properties![line-1]).called!();
                    }
                    else
                    {
                        display_menu((Displayable)menu.properties[line-1]);
                    }
                    menu = (Displayable)menu_call_stack.Pop();
                    header = get_header(menu);
                    line = line_call_stack.Pop();
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    //FIXME: 2 comments below are fucking stupid. Definitly not inntended behaviour
                    // if class is menu then the default action is to display the menu
                    // if class not menu then the default action is to call the edit or add function
                    if (menu.properties![line] is not Displayable)
                    {
                        //FIXME: index fucked!!!!!!!!!
                        //FIXME: open / enter lists with right arrow
                        Edit_or_Add_more_data(menu, menu.properties.FindIndex(x => x == menu.properties![line]));
                    } else {
                        menu_call_stack.Push(menu);
                        line_call_stack.Push(line);
                        if (((Displayable)menu.properties![line-1]).called != null)
                        {
                            ((Displayable)menu.properties![line-1]).called!();
                        }
                        else
                        {
                            display_menu((Displayable)menu.properties[line-1]);
                        }
                        menu = (Displayable)menu_call_stack.Pop();
                        header = get_header(menu);
                        line = line_call_stack.Pop();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Continue = false;
                }
            } while (Continue);
        }
        static void Edit_or_Add_more_data(Displayable father, int index)
        { 
            int column = 0;
            bool Continue = true;
            string[] options = new string[3] {"Add", "Edit" , "Remove"};
            Console.ResetColor(); Console.WriteLine();
            Console.SetCursorPosition(0, setting_rows + 1);
            do {
                Write(column == 0 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[0] , ConsoleColorExtension.colors["Default"] , "\t|\t" 
                , column == 1 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[1], ConsoleColorExtension.colors["Default"], "\t|\t"
                , column == 2 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[2], ConsoleColorExtension.colors["Default"]);
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (column <= 0)
                    {
                        column = options.Count() - 1;
                    }
                    else
                    {
                        column--;
                    }
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (column >= options.Count() - 1)
                    {
                        column = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
                if (keyInfo.Key == ConsoleKey.Enter) {
                    if (column == 0)
                    {
                        As_property(father, index);
                    }
                    else if (column == 1)
                    {
                        Edit_data(data);
                    }
                    else if (column == 2)
                    {
                        Remove_data(data);
                    }
                }
                if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.OemPlus)
                {
                    As_property(data);
                }
                if (keyInfo.Key == ConsoleKey.E)
                {
                    Edit_data(data);
                }
                if (keyInfo.Key == ConsoleKey.R || keyInfo.Key == ConsoleKey.Delete || keyInfo.Key == ConsoleKey.OemMinus)
                {
                    Remove_data(data);
                }                
                else if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Backspace)
                {
                    Continue = false;
                }
            } while (Continue);
            //edit or add more data
        }
        static void As_property(Displayable father, int index)
        {
            int column = 0;
            bool Continue = true;
            string[] options = new string[2] {"As Property", "As part of the List"};
            Console.ResetColor(); Console.WriteLine();
            Console.SetCursorPosition(0, setting_rows + 1);
            do {
                Write(column == 0 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[0] , ConsoleColorExtension.colors["Default"] , "\t|\t" 
                , column == 1 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[1], ConsoleColorExtension.colors["Default"]);
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (column == 0)
                    {
                        column = options.Count() - 1;
                    }
                    else
                    {
                        column--;
                    }
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (column == options.Count() - 1)
                    {
                        column = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter) {
                    if (column == 0)
                    {
                        //As property
                        if (data is Displayable) { 
                            //if properties is null then create it (null-coalescing assignment operator)
                            ((Displayable)data).properties ??= new List<object>();
                            ((Displayable)data).properties!.Add(Add_data());
                            Continue = false;
                        } else { 
                            Write(ConsoleColorExtension.colors["Error"], "This is not a Displayable object.\n",
                            ConsoleColorExtension.colors["Info"], "To write to this item you need to change it's type class to a Displayable one!");
                            //TODO: implemet a way to change existing data's class
                            keyInfo = Console.ReadKey(true);
                        }
                    }
                    else if (column == 1)
                    {
                        //As part of the List
                        Above_or_below(data);
                        Continue = false;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.P)
                {
                    if (data is Displayable) { 
                        //if properties is null then create it (null-coalescing assignment operator)
                        ((Displayable)data).properties ??= new List<object>();
                        ((Displayable)data).properties!.Add(Add_data());
                        Continue = false;
                    } else { 
                        Write(ConsoleColorExtension.colors["Error"], "This is not a Displayable object.\n",
                        ConsoleColorExtension.colors["Info"], "To write to this item you need to change it's type class to a Displayable one!");
                        //TODO: implemet a way to change existing data's class
                        keyInfo = Console.ReadKey(true);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    Above_or_below(data);
                    Continue = false;
                }                
                else if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Backspace)
                {
                    Continue = false;
                }
            } while (Continue);
            //add as property or as part of the list
        }
        static void Above_or_below(Displayable father, int index) 
        { 
            int column = 0;
            bool Continue = true;
            string[] options = new string[2] {"Above", "Below"};
            Console.ResetColor(); Console.WriteLine();
            Console.SetCursorPosition(0, setting_rows + 1);
            do {
                Write(column == 0 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[0] , ConsoleColorExtension.colors["Default"] , "\t|\t" 
                , column == 1 ? ConsoleColorExtension.colors["Selected"] : ConsoleColorExtension.colors["Default"], options[1], ConsoleColorExtension.colors["Default"]);
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (column == 0)
                    {
                        column = options.Count() - 1;
                    }
                    else
                    {
                        column--;
                    }
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (column == options.Count() - 1)
                    {
                        column = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter) {
                    if (column == 0)
                    {
                        //Above
                        //<v>
                        Add_data(data, true);
                        Continue = false;
                    }
                    else if (column == 1)
                    {
                        //Below
                        Add_data(data, false);
                        Continue = false;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.OemPlus)
                {
                    Add_data(data, true);
                    Continue = false;
                }
                else if (keyInfo.Key == ConsoleKey.B || keyInfo.Key == ConsoleKey.OemMinus)
                {
                    Add_data(data, false);
                    Continue = false;
                }                
                else if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Backspace)
                {
                    Continue = false;
                }
            } while (Continue);
            //add above or below
        }
        static object Add_data()
        {
            //return an object for exaple a string filled or a Displayable with .name and .classes filled
            object new_data = null;
            clear_and_reset_color();
            return new_data;
            //add data
        }
        static void Edit_data(object data)
        {
            //edit data
        }
        static void Remove_data(object data)
        {
            //remove data
        }

        //useless?
        static ConsoleColorExtension selected(object number, object number2){
            if (Convert.ToInt32(number) == Convert.ToInt32(number2))
            {
                return ConsoleColorExtension.colors["Selected"];
            }
            else
            {
                return ConsoleColorExtension.colors["Default"];
            }
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
                        new ConsoleColorExtension("Error", ConsoleColor.Red, ConsoleColor.Black),
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
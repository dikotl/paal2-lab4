using System.Diagnostics;
using System.Text;

namespace Lab4;

class Program
{
    static void Main(string[] args)
    {
        // Selector menu.
        while (true)
        {
            Console.WriteLine("""
            # List of Tasks
                1. Task 1: Generate Number Sequence (4 methods + performance test)
                2. Task 2: Palindrome/Reverse Words (string version, no punctuation)
                3. Task 2: Palindrome/Reverse Words (StringBuilder version, no punctuation)
                4. Task 2: Palindrome/Reverse Words (string version, with punctuation)
                5. Task 2: Palindrome/Reverse Words (StringBuilder version, with punctuation)
                6. Task 15: Check if all parenthesis are closed
                0. Exit
            """);

            Console.Write("Enter task number to run: ");

            switch (Console.ReadLine()!)
            {
            case "1":
                Task1_CheckPerformance();
                break;
            case "2":
                Task2_StringNoPunctuation();
                break;
            case "3":
                Task2_StringBuilderNoPunctuation();
                break;
            case "4":
                Task2_StringWithPunctuation();
                break;
            case "5":
                Task2_StringBuilderWithPunctuation();
                break;
            case "6":
                Task15();
                break;
            case "0":
                Console.WriteLine("Exiting program...");
                return;
            default:
                Console.WriteLine("Invalid choice. Please enter a number from the menu.");
                break;
            }
        }
    }

    //
    // Task 1: Generating Number Sequences.
    //

    record Task1Result(string MethodName, TimeSpan ExecutionTime, string Description);

    static void Task1_CheckPerformance()
    {
        Console.Write("Enter a positive integer n: ");

        int n;

        while (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
        {
            Console.WriteLine("Error! Invalid input");
        }

        Console.WriteLine($"\n# Generating sequence for n = {n}\n");

        var results = new List<Task1Result>();
        var stopwatch = new Stopwatch();

        // Method 1
        stopwatch.Restart();
        string result1 = Task1_StringAppendEnd(n);
        stopwatch.Stop();
        results.Add(new Task1Result("StringAppendEnd", stopwatch.Elapsed, "string, appending 1 to n using +="));
        Console.WriteLine($"Result 1: {result1}");

        // Method 2
        stopwatch.Restart();
        string result2 = Task1_StringInsertStart(n);
        stopwatch.Stop();
        results.Add(new Task1Result("StringInsertStart", stopwatch.Elapsed, "string, inserting n to 1 using +"));
        Console.WriteLine($"Result 2: {result2}");

        // Method 3
        stopwatch.Restart();
        string result3 = Task1_StringBuilderAppendEnd(n);
        stopwatch.Stop();
        results.Add(new Task1Result("StringBuilderAppend", stopwatch.Elapsed, "StringBuilder, appending 1 to n using Append()"));
        Console.WriteLine($"Result 3: {result3}");

        // Method 4
        stopwatch.Restart();
        string result4 = Task1_StringBuilderInsertStart(n);
        stopwatch.Stop();
        results.Add(new Task1Result("StringBuilderInsert", stopwatch.Elapsed, "StringBuilder, inserting n to 1 using Insert()"));
        Console.WriteLine($"Result 4: {result4}");

        PrintPerformanceTable(results);
    }

    static void PrintPerformanceTable(List<Task1Result> results)
    {
        Console.WriteLine("\n# Performance Comparison");

        int nameWidth = results.Max(r => r.MethodName.Length) + 2;
        int timeWidth = results.Max(r => FormatTimeSpan(r.ExecutionTime).Length) + 2;
        int descWidth = results.Max(r => r.Description.Length) + 2;

        // Header.
        string header = $"| {"Method".PadRight(nameWidth)}| {"Time".PadRight(timeWidth)}| {"Description".PadRight(descWidth)}|";
        string separator = new('-', header.Length);
        Console.WriteLine(separator);
        Console.WriteLine(header);
        Console.WriteLine(separator);

        // Rows.
        foreach (var result in results)
        {
            Console.WriteLine($"| {result.MethodName.PadRight(nameWidth)}| {FormatTimeSpan(result.ExecutionTime).PadRight(timeWidth)}| {result.Description.PadRight(descWidth)}|");
        }

        Console.WriteLine(separator);
    }

    static string FormatTimeSpan(TimeSpan ts)
    {
        if (ts.TotalSeconds >= 1)
            return $"{ts.TotalSeconds:F4} s";

        if (ts.TotalMilliseconds >= 1)
            return $"{ts.TotalMilliseconds:F4} ms";

        return $"{ts.TotalMilliseconds * 1000000:F0} ns";
    }

    static string Task1_StringAppendEnd(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);

        var buf = "1";

        for (int i = 2; i <= n; ++i)
        {
            buf += $" {i}";
        }

        return buf;
    }

    static string Task1_StringInsertStart(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);

        var buf = $"{n}";

        for (int i = n - 1; i >= 1; --i)
        {
            buf = $"{i} {buf}";
        }

        return buf;
    }

    static string Task1_StringBuilderAppendEnd(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);

        var buf = new StringBuilder("1");

        for (int i = 2; i <= n; i++)
        {
            buf.Append(' ');
            buf.Append(i);
            // Or this.
            /* sb.Append($"{i} "); */
        }

        return buf.ToString();
    }

    static string Task1_StringBuilderInsertStart(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);

        var buf = new StringBuilder($"{n}");

        for (int i = n - 1; i >= 1; i--)
        {
            buf.Insert(0, ' ');
            buf.Insert(0, i);
            // Or this.
            /* sb.Insert(0, $"{i} "); */
        }

        return buf.ToString();
    }

    //
    // Task 2: Palindromes and Reversals. Helpers.
    //

    // Checks if a word is a palindrome.
    //
    // Details:
    // - case-insensitive
    // - ignores non-letters/digits for punctuation version.
    static bool IsPalindrome(string word, bool ignorePunctuation = false)
    {
        // Empty string is arguably a palindrome.
        if (string.IsNullOrEmpty(word))
            return true;

        int left = 0;
        int right = word.Length - 1;

        while (left < right)
        {
            // Skip non-alphanumeric characters if required.
            if (ignorePunctuation)
            {
                while (left < right && !char.IsLetterOrDigit(word[left]))
                    ++left;

                while (right > left && !char.IsLetterOrDigit(word[right]))
                    --right;
            }

            // Compare characters case-insensitively.
            if (char.ToLowerInvariant(word[left]) != char.ToLowerInvariant(word[right]))
            {
                return false;
            }

            ++left;
            --right;
        }

        return true;
    }

    static string FlipCase(string word)
    {
        var buf = new StringBuilder(word.Length);

        foreach (char c in word)
        {
            buf.Append(
                char.IsLetter(c)
                    ? char.IsUpper(c)
                        ? char.ToLowerInvariant(c)
                        : char.ToUpperInvariant(c)
                    : c // Keep non-letters as they are.
            );
        }

        return buf.ToString();
    }

    static string FlipWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        return string.Join("", word.ToCharArray().Reverse());
    }

    //
    // Task 2: Palindromes and Reversals. Implementations.
    //

    static string ProcessWordsUsingString(string input, bool ignorePunctuation)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Split into words, preserving multiple spaces if necessary
        // (though problem implies single spaces).
        var words = input.Split(' ', StringSplitOptions.None); // Use None to avoid removing empty entries if needed
        var result = "";

        for (int i = 0; i < words.Length; ++i)
        {
            var word = words[i];
            var processedWord = string.IsNullOrEmpty(word)
                ? word
                : IsPalindrome(word, ignorePunctuation)
                    ? FlipCase(word)
                    : FlipWord(word);

            if (i > 0)
                result += " ";

            result += processedWord;
        }

        return result;
    }

    static string ProcessWordsUsingStringBuilder(string input, bool ignorePunctuation)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Split into words, preserving multiple spaces if necessary
        // (though problem implies single spaces).
        var words = input.Split(' ', StringSplitOptions.None); // Use None to avoid removing empty entries if needed
        var result = new StringBuilder(words.Length);

        for (int i = 0; i < words.Length; ++i)
        {
            var word = words[i];
            var processedWord = string.IsNullOrEmpty(word)
                ? word
                : IsPalindrome(word, ignorePunctuation)
                    ? FlipCase(word)
                    : FlipWord(word);

            if (i > 0)
                result.Append(' ');

            result.Append(processedWord);
        }

        return result.ToString();
    }

    static void Task2_StringNoPunctuation()
    {
        Console.WriteLine("\nEnter a sentence (no punctuation):");

        string result = ProcessWordsUsingString(input: Console.ReadLine()!, ignorePunctuation: false);

        Console.WriteLine($"Result (string, no punctuation): {result}");
    }

    static void Task2_StringBuilderNoPunctuation()
    {
        Console.WriteLine("\nEnter a sentence (no punctuation):");

        string result = ProcessWordsUsingStringBuilder(input: Console.ReadLine()!, ignorePunctuation: false);

        Console.WriteLine($"Result (StringBuilder, no punctuation): {result}");
    }

    static void Task2_StringWithPunctuation()
    {
        Console.WriteLine("\nEnter a sentence (can include punctuation):");

        string result = ProcessWordsUsingString(input: Console.ReadLine()!, ignorePunctuation: true);

        Console.WriteLine($"Result (string, with punctuation): {result}");
    }

    static void Task2_StringBuilderWithPunctuation()
    {
        Console.WriteLine("\nEnter a sentence (can include punctuation):");

        string result = ProcessWordsUsingStringBuilder(input: Console.ReadLine()!, ignorePunctuation: false);

        Console.WriteLine($"Result (StringBuilder, with punctuation): {result}");
    }

    //
    // Task 15: Check if all parenthesis are closed.
    //

    static void Task15()
    {
        Console.Write("\nInput something: ");

        var input = Console.ReadLine()!;
        var isValidInput = Task15_Impl(input);

        Console.WriteLine(
            isValidInput
                ? "\nInput is valid\n"
                : "\nInput is invalid\n"
        );
    }

    static bool Task15_Impl(string input)
    {
        int parensCount = 0;

        foreach (char ch in input)
        {
            if (ch == '(')
            {
                ++parensCount;
            }
            else if (ch == ')')
            {
                if (--parensCount < 0)
                {
                    return false;
                }
            }
        }

        return parensCount == 0;
    }
}

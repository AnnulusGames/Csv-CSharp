namespace Csv.Formatters;

public static class CompositeFormatterProvider
{
    public static ICsvFormatterProvider Create(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2)
    {
        return new Formatter2(provider1, provider2);
    }

    public static ICsvFormatterProvider Create(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3)
    {
        return new Formatter3(provider1, provider2, provider3);
    }

    public static ICsvFormatterProvider Create(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3, ICsvFormatterProvider provider4)
    {
        return new Formatter4(provider1, provider2, provider3, provider4);
    }

    public static ICsvFormatterProvider Create(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3, ICsvFormatterProvider provider4, ICsvFormatterProvider provider5)
    {
        return new Formatter5(provider1, provider2, provider3, provider4, provider5);
    }

    public static ICsvFormatterProvider Create(params ICsvFormatterProvider[] providers)
    {
        return new Formatters(providers);
    }

    sealed class Formatter2(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2) : ICsvFormatterProvider
    {
        public ICsvFormatter<T>? GetFormatter<T>()
        {
            var formatter = provider1.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider2.GetFormatter<T>();
            return formatter;
        }
    }

    sealed class Formatter3(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3) : ICsvFormatterProvider
    {
        public ICsvFormatter<T>? GetFormatter<T>()
        {
            var formatter = provider1.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider2.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider3.GetFormatter<T>();
            return formatter;
        }
    }

    sealed class Formatter4(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3, ICsvFormatterProvider provider4) : ICsvFormatterProvider
    {
        public ICsvFormatter<T>? GetFormatter<T>()
        {
            var formatter = provider1.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider2.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider3.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider4.GetFormatter<T>();
            return formatter;
        }
    }

    sealed class Formatter5(ICsvFormatterProvider provider1, ICsvFormatterProvider provider2, ICsvFormatterProvider provider3, ICsvFormatterProvider provider4, ICsvFormatterProvider provider5) : ICsvFormatterProvider
    {
        public ICsvFormatter<T>? GetFormatter<T>()
        {
            var formatter = provider1.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider2.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider3.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider4.GetFormatter<T>();
            if (formatter != null) return formatter;
            formatter = provider5.GetFormatter<T>();
            return formatter;
        }
    }

    sealed class Formatters(ICsvFormatterProvider[] providers) : ICsvFormatterProvider
    {
        public ICsvFormatter<T>? GetFormatter<T>()
        {
            foreach (var provider in providers)
            {
                var formatter = provider.GetFormatter<T>();
                if (formatter != null) return formatter;
            }

            return null;
        }
    }
}
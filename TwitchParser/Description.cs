namespace TwitchParser
{
    public class Description
    {
        public static string GetEnglish(string game)
        {
            return $"Top clips {game} from past day.";
        }
        public static string GetRussian(string game)
        {
            return $"Лучшие моменты с {game} за прошлый день.";
        }
        public static string GetIndigoDescription(string game)
        {
	        return $"Лучшие моменты с GTA 5 за прошлый день. \n \nЗаходи на сервер Indigo по промокоду 'indigomoments' и получай бонусы в виде: \nBronze VIP на 3 дня, 10000$ и 300IC! а так же участвуй в скором розыгрыше!";
        }

    }
}
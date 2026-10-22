// QRLab.Image/Generation/QRCodeGenerator.cs
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace QRLab.Image.Generation
{
    public class QRCodeGenerator
    {
        // Константы для выбора версии QR-кода
        private const int MinVersion = 1;
        private const int MaxVersion = 10;
        // Константы для коррекции ошибок
        private static readonly ErrorCorrectionLevel[] ErrorCorrectionLevels =
            { ErrorCorrectionLevel.L, ErrorCorrectionLevel.M, ErrorCorrectionLevel.Q, ErrorCorrectionLevel.H };
        // Генерация QR-кода
        public static byte[] GenerateQRCode(string input, ErrorCorrectionLevel level = ErrorCorrectionLevel.M)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string cannot be empty");
            // Выбор версии QR-кода
            int version = SelectVersion(input.Length);
            // Проверка корректности уровней коррекции
            if (!ErrorCorrectionLevels.Contains(level))
                throw new ArgumentException("Invalid error correction level");
            // Создание QR-кода
            var qrCodeData = new QRCodeGenerator().GenerateQRCode(input, level);
            var qrCode = qrCodeData.GetGraphic(20, Brushes.Black, Brushes.White, 20, 20, 10, 0, 0);
            // Сохранение в byte[]
            using (var memoryStream = new System.IO.MemoryStream())
            {
                qrCode.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
        // Автоматический выбор версии QR-кода
        private static int SelectVersion(int inputLength)
        {
            // Формула выбора версии (пример)
            int version = (int)Math.Min(MaxVersion, Math.Max(MinVersion,
                (inputLength / 150) + (inputLength % 150 != 0 ? 1 : 0)));
            if (version > MaxVersion)
                throw new InvalidOperationException("Input too long for QR code");
            return version;
        }
        // Генерация синтетических образцов
        public static byte[] GenerateSampleData(string sampleText, int version = 10)
        {
            // Нужно добавить логику генерации демонстрационных образцов
            return GenerateQRCode(sampleText, ErrorCorrectionLevel.Q); // Пример с коррекцией Q
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab5_ex1.Objects;

namespace lab5_ex1
{
    public partial class Form1 : Form
    {
        public int score = 0;
        List<BaseObject> objects = new();
        Player player;
        Marker marker;
        GreenCircle greencircle;
        GreenCircle greencircle2;
        public Form1()
        {
            InitializeComponent();

            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);
            greencircle = new GreenCircle(0, 0, 0);
            greencircle2 = new GreenCircle(0, 0, 0);


            // добавил реакцию на пересечение с маркером
            player.OnMarkerOverlap += (m) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок добрался до маркера.\n" + txtLog.Text;
                objects.Remove(m);
                marker = null;
            };
            // зеленый шар
            player.OnGreenCircleOverlap += (c) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок успел активировать пикап!\n" + txtLog.Text;
                GenerateCircle(c);
                //greencircle.time = 150;
                score++;
                ScoreOutput();
            };

            marker = new Marker(pbMain.Width / 2 + 1, pbMain.Height / 2 + 1, 0);
            objects.Add(marker);
            objects.Add(player);
            objects.Add(greencircle);
            objects.Add(greencircle2);
  
        }

        private void ScoreOutput()
        {
            label1.Text = "Счет: " + score;
        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; // вытащили объект графики из события

            updatePlayer(); // сюда, теперь сначала вызываем пересчет игрока

            g.Clear(Color.White); // фон

            // меняю тут objects на objects.ToList()
            // это будет создавать копию списка
            // и позволит модифицировать оригинальный objects прямо из цикла foreach
            // пересчитываем пересечения
            foreach (var obj in objects.ToList())
            {
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj);
                    obj.Overlap(player);
                }
            }

            // рендерим объекты
            foreach (var obj in objects)
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }
        }

        private void updatePlayer()
        {
            // тут добавляем проверку на marker не нулевой
            if (marker != null)
            {
                // рассчитываем вектор между игроком и маркером
                float dx = marker.X - player.X;
                float dy = marker.Y - player.Y;

                // находим его длину
                float length = MathF.Sqrt(dx * dx + dy * dy);
                dx /= length; // нормализуем координаты
                dy /= length;

                // по сути мы теперь используем вектор dx, dy
                // как вектор ускорения, точнее даже вектор притяжения
                // который притягивает игрока к маркеру
                // 0.5 просто коэффициент который подобрал на глаз
                // и который дает естественное ощущение движения
                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                // расчитываем угол поворота игрока 
                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }
            // тормозящий момент,
            // нужен чтобы, когда игрок достигнет маркера произошло постепенное замедление
            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            // пересчет позиция игрока с помощью вектора скорости
            player.X += player.vX;
            player.Y += player.vY;
        }

        private void GenerateCircle(GreenCircle сircle)
        {
            Random random = new Random();
            сircle.X = random.Next() % 530;
            сircle.Y = random.Next() % 370;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var r = new Random();
            foreach (var obj in objects.ToList())
            {
                if (obj is GreenCircle greencircle)
                {
                    greencircle.time--;
                    if (greencircle.time <= 0)
                    {
                        GenerateCircle(greencircle);
                        greencircle.time = 150;
                    }
                }
            }
            // запрашиваем обновление pbMain
            // это вызовет метод pbMain_Paint по-новой
            pbMain.Invalidate();
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            // тут добавил создание маркера по клику если он еще не создан
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker); // и главное не забыть пололжить в objects
            }

            marker.X = e.X;
            marker.Y = e.Y;
        }
    }
}

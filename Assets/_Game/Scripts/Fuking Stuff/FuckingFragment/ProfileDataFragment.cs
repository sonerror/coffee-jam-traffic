using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName = "Stuff/Data Fragment/Profile Fragment", fileName = "Profile  Fragment")]
public class ProfileDataFragment : DataFragment
{
    public static ProfileDataFragment cur;
    public Sprite[] spr_avatars;
    public Sprite[] spr_frames;

    public Data gameData;

    [HideInInspector] public List<string> names = new List<string>
        {
            "たかし", "あい", "ひろし", "まさみ", "たけし",
            "けん", "さくら", "あかり", "はるか", "いちろう",
            "さとみ", "じゅん", "みずき", "けいこ", "ゆうじ",
            "さやか", "なおき", "まい", "かずお", "こうじ",
            "김민준", "이서윤", "박지후", "최현우", "강예린",
            "이정호", "신유리", "윤도현", "한수진", "정은지",
            "박찬호", "백현서", "손미진", "유경민", "황지성",
            "고은비", "문석진", "오세영", "임정우", "서준호",
            "Juan", "María", "Carlos", "Sofía", "Miguel", "Alejandro", "Ana", "Lucía", "Pablo", "Carmen",
            "Javier", "Sara", "Manuel", "Laura", "Antonio", "Beatriz", "Francisco", "Elena", "Fernando", "Alicia",
            "Дмитрий", "Анастасия", "Сергей", "Ольга", "Алексей",
            "Екатерина", "Иван", "Мария", "Андрей", "Татьяна",
            "Николай", "Наталья", "Владимир", "Елена", "Михаил",
            "Анна", "Виктор", "Светлана", "Юрий", "Лидия",
            "王伟", "李娜", "张敏", "刘洋", "陈杰", "杨婷",
            "赵磊", "周欣", "黄勇", "吴佳", "孙峰", "徐静",
            "郑军", "马丽", "谢强", "胡欣", "郝伟", "蒋丽",
            "محمد", "فاطمة", "علي", "عائشة", "يوسف", "نور", 
            "إبراهيم", "ليلى", "خالد", "مريم", "حسين", "سارة",
            "عمر", "زينب", "أحمد", "رنا", "صالح", "ريم", 
            "عبد الله", "هبة",
            "Michael", "Emily", "Christopher", "Jessica", "Joshua", "Amanda", "Daniel", "Jennifer", "James", "Ashley",
            "Matthew", "Sarah", "Andrew", "Elizabeth", "David", "Samantha", "Justin", "Megan", "Ryan", "Lauren",
            "Jean", "Marie", "Pierre", "Claire", "Jacques", "Sophie", "Louis", "Isabelle", "Paul", "Amélie",
            "Philippe", "Camille", "Bernard", "Charlotte", "Alain", "Lucie", "Georges", "Pauline", "André", "Léa",
            "김수현", "한지원", "정민재", "이혜진", "박준수",
            "李强", "王雪", "张军", "陈萍", "赵敏",
            "Олег", "Евгения", "Артём", "Ирина", "Валерий",
            "José", "Isabel", "Mario", "Victoria", "Alberto",
            "Oliver", "Amelia", "Henry", "Charlotte", "Samuel",
            "François", "Julie", "Luc", "Nathalie", "Victor",
            "Lukas", "Emma", "Maximilian", "Sophia", "Felix",
            "Marco", "Giulia", "Luca", "Francesca", "Giorgio",
            "Pedro", "Inês", "João", "Sofia", "Tiago",
            "Arjun", "Aishwarya", "Raj", "Priya", "Vikram",
            "أمل", "محمود", "فادي", "صفية", "أسامة",
            "Νίκος", "Ελένη", "Γιώργος", "Αλεξάνδρα", "Δημήτρης",
            "Gabriel", "Rafael", "Fernanda", "Bruna", "Thiago",
            "Леонид", "Алиса", "Константин", "Нина", "Егор",
            "최지훈", "유소영", "김태우", "박하늘", "이지은",
            "สมชาย", "อรทัย", "ปกรณ์", "วิรัตน์", "กมล",
            "Ngọc", "Quốc", "Bảo", "Lan", "Huy",
            "Putra", "Dewi", "Wayan", "Agung", "Rani",
            "Juan", "Maria", "Jose", "Angel", "Ricardo",
            "Chinedu", "Ngozi", "Ifeanyi", "Adebayo", "Chioma",
            "Carlos", "Santiago", "Luisa", "Alejandro", "Valeria",
            "Piotr", "Anna", "Krzysztof", "Ewa", "Marek",
            "Lars", "Karin", "Björn", "Anna-Lena", "Erik",
            "Ola", "Solveig", "Kjetil", "Ingrid", "Eirik",
            "Mikko", "Aino", "Janne", "Liisa", "Petri",
            "Ahmet", "Fatma", "Mehmet", "Zeynep", "Can",
            "Ali", "Fatemeh", "Reza", "Sara", "Hossein",
            "Mwangi", "Amina", "Omondi", "Wanjiru", "Mutua",
            "Lucía", "Mateo", "Florencia", "Joaquín", "Camila",
            "Fernanda", "Ignacio", "Sofía", "Benjamín", "Valentina",
            "Андрій", "Олена", "Микола", "Катерина", "Тарас",
            "Ionut", "Elena", "Stefan", "Andreea", "Mihai",
            "Abdul", "Ayesha", "Mohammad", "Zara", "Hassan",
            "David", "Miriam", "Yosef", "Noa", "Avraham",
            "Abebe", "Alem", "Bekele", "Desta", "Tigist",
            "Aroha", "Tane", "Hemi", "Rangi", "Mahuika",
            "Sibusiso", "Thandeka", "Lerato", "Mfundo", "Bongani",
            "Nikolas", "Maria", "Katerina", "Yannis", "Eleni",
            "Jón", "Sigríður", "Björg", "Einar", "Guðrún",
            "Aoife", "Conor", "Niamh", "Oisín", "Róisín",
            "João", "Catarina", "Miguel", "Inês", "Francisco",
            "István", "Zoltán", "Ágnes", "László", "Judit",
            "Miloš", "Jelena", "Vuk", "Dragana", "Stefan",
            "Ivan", "Ana", "Marko", "Petra", "Luka",
            "Georgi", "Ivanka", "Radoslav", "Stoyan", "Desislava",
            "Nino", "Giorgi", "Tamar", "Levan", "Elene",
            "Mansur", "Dilbar", "Ulugbek", "Shirin", "Javlon",
            "Andrés", "Gabriela", "Luis", "Patricia", "Rafael",
            "Agustina", "Rodrigo", "Victoria", "Diego", "Martín",
            "Elle", "Niillas", "Áilu", "Risten", "Oskari",
            "Karim", "Layla", "Tarek", "Nadia", "Samir",
            "Bikram", "Sita", "Prakash", "Laxmi", "Manoj",
            "Chenda", "Sophea", "Piseth", "Dara", "Sothy",
            "Ola", "Ingrid", "Sven", "Astrid", "Kari",
            "Frederik", "Sofie", "Jens", "Lars", "Marie",
            "Juhani", "Emilia", "Tapio", "Veera", "Matti",
            "Lucas", "Lotte", "Maarten", "Annelies", "Svenja",
            "Petr", "Jana", "Václav", "Lenka", "Karel",
            "Marek", "Zuzana", "Juraj", "Tatiana", "Michal",
            "Max", "Chantal", "Tom", "Colette", "Henri",
            "Arben", "Ilir", "Ariana", "Luan", "Edi",
            "Goran", "Ana", "Stefan", "Ivana", "Nikola",
            "Amir", "Selma", "Eldar", "Alma", "Denis",
            "Hayk", "Mariam", "Vardan", "Arpine", "Narek",
            "Rasim", "Zahra", "Elchin", "Leyla", "Togrul",
            "Carlos", "María", "Juan", "Paola", "Hernán",
            "Felipe", "Camila", "Sebastián", "Valentina", "Ignacio",
            "Luca", "Maria", "Rafael", "Sophia", "Leo",
            "Keshawn", "Anya", "Shane", "Janelle", "Imran",
            "Viliame", "Losalini", "Mosese", "Adi", "Eroni",
            "Kaloris", "Selina", "Moses", "Rosina", "Petro",
            "Sione", "Mele", "Lopeti", "Ana", "Feki"
        };


#if UNITY_EDITOR
    private void OnEnable()
    {
        cur = this;
    }
#else
    private void Awake()
    {
        cur = this;
    }
#endif

    public void CapNhatTen()
    {
        if(gameData.chuaCoTen)
        {
            gameData.chuaCoTen = false;
            gameData.userName  = "User" + UnityEngine.Random.Range(99999, 99999999).ToString("00000000");
        }
    }


    public override void Load()
    {
        if (!LoadData(ref gameData, key)) ResetData();
    }

    public override void Save()
    {
        SaveData(gameData, key);
    }

    public override void ResetData()
    {
        gameData = new Data();
    }


    [Serializable]
    public class Data
    {
        public bool chuaCoTen = true;
        public int idChoose = 0;
        public int idAvatar = 0;
        public int idFrame = 0;
        public string userName = "User251123164";
    }
}
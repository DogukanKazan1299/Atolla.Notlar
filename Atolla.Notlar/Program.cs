using System.Text.RegularExpressions;


#region 1-IEnumerable<T> query;

//Aşağıdaki kod satırnın çıktısı ne olur ? Sonuç null.
//Neden, çünkü “IEnumerable” query hemen çalıştırılmaz. “numbers.Clear()” ile dizi temizlendikten sonra, “foreach” içinde çağrıldığı için, null değer döner.


var numbers = new List<int>() { 1, 2 };
IEnumerable<int> query = numbers.Select(n => n * 10);
numbers.Clear();
Console.WriteLine("IEnumerable:");
foreach (int n in query)
{
	Console.WriteLine(n + "|");
}

//ÇIKTI: IEnumerable:

#endregion

#region 

//Peki aynı sorguyu aşağıdaki gibi List<T> ile yapılsa idi, sonuc ne olurdu?
//“List<T>()” sorguları hemen işletildiği için, sorgu sonucu listenin silinmesinden etkilenmez ve sonuçlar gelirdi.

var numbers2 = new List<int>() { 1, 2 };
List<int> timesTen = numbers2.Select(n => n * 10).ToList();
numbers2.Clear();
Console.WriteLine("List:");
foreach (int n in timesTen)
{
	Console.WriteLine(n + "|");
}

//ÇIKTI : List : 10 | 20
#endregion


#region
//Son olarak, aşağıdaki sorgunun sonucu ne olurdu?
//IEnumerable, gerçekten çağrıldıktan sonra çalıştırıldığı için, sorguya sonradan müdahale edilebilmiş ve “factor” katsayısının ilk hali ile değil, son hali ile ilgili sorgu çağrılmıştır.

var numbers3 = new List<int>() { 1, 2 };
int factor = 10;
IEnumerable<int> query1 = numbers3.Select(n => n * factor);

factor = 20;
Console.WriteLine("IEnumerable:");
foreach (int n in query1)
{
	Console.WriteLine(n + "|");
}

//ÇIKTI : IEnumerable : 20 | 40

#endregion

#region

//Aşağıda görüldüğü gibi Linq ile “where” koşulları peş peşe eklenerek ilgili sorgu çağrılabilir. Amaç cümle içinden “a,t,u,l,e” harflerinin çıkarılmasıdır.
IEnumerable<char> query2 = "Cut the night with the light";
query2 = query2.Where(c => c != 'a');
query2 = query2.Where(c => c != 't');
query2 = query2.Where(c => c != 'u');
query2 = query2.Where(c => c != 'l');
query2 = query2.Where(c => c != 'e');

foreach (char c in query2)
{
	Console.Write(c);
}


//ÇIKTI : C h nigh wih h igh

#endregion

#region
//Eğer aşağıdaki gibi bir sorgu yazılır ise de, yine aynı sonuç alınır. “Where”‘ler peş peşe sona eklenir. Ve ilgili query en son çalıştırılarak, yine aynı sonuç alınır.
//Not: ” query = query.Where( c => c != excepts[i] ) ”
//  Şeklinde kod yazılsa idi, “i” değeri son olarak “5” değerini alacağı ve query enson foreach anında çağrılacağı için, “IndexOutOfRangeException” hatası alınacakdı.
//Çünkü excepts[5] dizisinin, 5. elemanı yok.

IEnumerable<char> query3 = "Cut the night with the light";
string excepts = "atule";
for (int i = 0; i < excepts.Length; i++)
{
	char except = excepts[i];
	query = query.Where(c => c != except);
}
foreach (char c in query3)
	Console.Write(c);


//ÇIKTI : Cut the night with the light

#endregion



#region 2-Anonymous Types ve “into” Keyword

//Aşağıdaki örnekde görüldüğü gibi,
//“names” dizisi üzerinde “select new{ }” ile anonymous tipinde “Original” ve “Change” propertylerine sahip bir Liste üzerinde işlem yapılarak,
//“into” keyword’ü ile “temp” değişkenine atanmış ve ek filitrelemeler, bu atanan “temp” üzerinden yapılmıştır.

//Kısaca bir liste üzerinde, kendisi ve üzerinde işlem yapılacak eşleniği şeklinde 2 property olarak oluşturulmuş
//eşleneği üzerinde çeşitli işlemler yapılıp, filitreden geçirilmiştir. Bu filitreleri geçen elemanların da orginal hali
//ekrana bastırılmıştır.


string[] names = { "Bora", "Engin", "Selcuk", "Burak", "Veli" };
var query4 =
	from n in names
	select new
	{
		Original = n,
		Change = n.Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "")
	}
	into temp
	where temp.Change.Length > 2
	select temp.Original;

foreach (string c in query4)
	Console.WriteLine(c);

//ÇIKTI : Engin Burak Selçuk


#endregion

#region

//4-LET KEYWORD

//Aşağıda görüldüğü gibi, uzunluğu 3’den büyük isim sorgusu “let” ile “u” değişkenine büyük harfe çevrilerek aktarılmış, daha sonra da “Y” harfi ile bitenler sorgulanmıştır.

string[] names1 = { "Tom", "Dick", "Harry", "Mary", "Jay" };
IEnumerable<string> query5 =
	from n in names1
	where n.Length > 3
	let u = n.ToUpper()
	where u.EndsWith("Y")
	select u;
foreach (string name in query5)
{
	Console.WriteLine(name);
}

//ÇIKTI : Harry Mary

#endregion


# region 5-Indexed Filtering

//Aşağıda görüldüğü gibi, satır index’i where koşulunda “i” ile çekilmiştir. Çift olan satırlar ( i % 2 ==0) çekilip, tek satırlar filitrelenmiştir.

string[] names2 = { "Bora", "Engin", "Selcuk", "Burak", "Veli" };
IEnumerable<string> query6 = names2.Where((n, i) => i % 2 == 0);
foreach (string c in query6)
	Console.WriteLine(c);

//ÇIKTI : Bora Selçuk Veli

#endregion


# region 6-SelectMany

//Aşağıdaki örnekde, bir dizi elemanı kendisi ile cross match olmaktadır. Aynı Ad ve Soyad, iki kere match olmasın diye, “n != n2” koşulu konulmuştur.

string[] names3 = { "Bora", "Judy", "Bill" };
IEnumerable<string> query7 =
	from n in names3
	from n2 in names3
	where n != n2
	select n + " vs " + n2;
foreach (string name in query7)
{
	Console.WriteLine(name);
}

//ÇIKTI : Bora vs Judy   Bora vs Bill   Judy vs Bora   Judy vs Bill   Bill vs Bora   Bill vs Judy
#endregion


#region
//Aşağıdaki örnekde, aslında iç içe 2 döngü bulunmaktadır.
//Öncelikle tam isim listesi çekilir.
//Sonra SelectMany => ile Cross Match yapılmaktadır.
//Yani herbir isim, “fullName.Split()” ile ad, soyad olarak 2 boyutlu bir diziye dönüştürülür, ve herbir eleman yani “ad” ve “soyad” tek tek “tam ad” ile matchlenir.
//Örn : “{ [Anne , Anne Williams], [Williams, Anne Williams], [John, John Fred], [Fred, Jhon Fred ]}” gibi.
//Aynı matchlemenin, ad ve soyad için 2 kere tekrarlanmaması amacı ile, “fullName.Split()[0] == name” filitresi konulmuştur.
//Böylece ilgili eşlemenin, sadece “ad” için yapılması sağlanmıştır. Soyad için bir daha tekrarlanmamıştır.

string[] fullNames = { "Anne Williams", "John Fred", "Sue Green" };
IEnumerable<string> query8 =
	from fullName in fullNames
	from name in fullName.Split()
	where fullName.Split()[0] == name
	select name + " : " + fullName + "'den geliyor";
foreach (string name in query8)
{
	Console.WriteLine(name);
}


//ÇIKTI : Anne : Anne Williams'den geliyor    John : John Fred'den geliyor    Sue: Sue Green'den geliyor
#endregion


#region 7-) GroupBy Count
//Aşağıdaki örnekde, aynı olan hayvan sayısına göre bir gruplama yapılmış ve tekrar eden hayvan sayısına göre,  büyükten küçüğe doğru sıralanmıştır.
//Ekran çıktısı olarak, hayvan adı ve toplam tekrarlama sayısı yazılmıştır.

string[] votes = { "Dogs", "Cats", "Cats", "Dogs", "Dogs" };
var query9 = votes.GroupBy(v => v = v).OrderByDescending(t => t.Count()).Select(t2 => new { key = t2.Key, count = t2.Count() });
foreach (var animal in query9)
{
	Console.WriteLine($"{animal.key} : {animal.count}");
}

//ÇIKTI : Dogs : 3   Cats : 2

#endregion
# C? - dokumentacja wstępna języka

## Informacje ogólne

**C?** (czytane *C-null*) jest **interpretowanym, proceduralnym językiem wysokiego poziomu**, który swoją składnią przypomina języki C, C++ i C#. Główną cechą charakterystyczną języka C? jest **domyślna nullowalność wszystkich typów danych** (w tym typów prymitywnych) i jednoczesna **kontrola odwołań do wartości *null***.

Cele przyświecające powstaniu języka C?:
- połączenie **możliwości szybkiego pisania programów w stylu skryptowym** znanym z języków Python lub JavaScript razem z **przejrzystością, spójnością składni oraz bardzo czytelnymi konwencjami nazewniczymi** znanymi z języka C#, przez co język C? będzie językiem przyjaznym programiście, umożliwiającym szybkie pisanie programów
- **wbudowanie w język możliwości dodania informacji zwrotnej o powodzeniu wykonywanych w programie operacji**, co jest mechanizmem znanym z języka Rust

### Ogólne zestawienie wymagań funkcjonalnych

- Język **interpretowany**.
- Paradygmat **proceduralny**.
- **Statyczne i silne typowanie.** To wymaganie implikuje możliwość rzutowania jednych typów danych na inne, jak i również niejawnych konwersji przy operacjach na typach liczbowych o różnych zakresach wielkości.
- **Podstawowe operacje matematyczne** na typach liczbowych (dodawanie, odejmowanie, mnożenie, dzielenie, priorytetyzacja operacji z użyciem nawiasów).
- Udostępnienie operacji **konkatenacji łańcuchów znakowych** oraz **odwoływania się do konkretnych znaków w łańcuchu**.
- Dostępne są **instrukcje warunkowe *if-else*, pętla *while***.
- Możliwość **dodawania komentarzy jednolinijkowych**
- **Programista może tworzyć własne funkcje**, w których może definiować zmienne lokalne.
- Struktura kodu napisanego w języku C? przypomina tę znaną z języka C/C++.
- **Domyślna akceptacja wartości *null*** przez wszystkie typy, w tym wszystkie typy prymitywne.
- Wbudowana **kolekcja danych (słownik)**, będąca przykładem typu złożonego, posiadająca własny interfejs umożliwiający dodawanie, usuwanie, wyszukiwanie i sortowanie elementów. **Klucz w takim słowniku nie może mieć wartości *null***.
- **Sygnalizowanie błędów przy próbie operacji na wartości *null***.
- **Możliwość obsługi wyjątków** w blokach *try-catch*, znanych z innych języków programowania. Wyjątki w C? są jedynie komunikatami tekstowymi (tj. można "rzucać stringa")
- **Możliwość importowania funkcji z innych plików**, na takiej samej zasadzie jak w przypadku języka Python.

### Wymagania niefunkcjonalne

- **Wieloplatformowość** - interpreter dla języka C? będzie dostępny na platformy Windows, macOS oraz Linux.
- **Niezawodność interpretera** - każda niestandardowa sytuacja będzie obsługiwana w odpowiedni sposób przez interpreter, przez co niemożliwe będzie, aby interpreter w nieoczekiwany sposób zakończył swoje działanie.
- **Przyjazność programiście** - składnia oraz konwencje nazewnicze wykorzystywane w języku C? będą wspierać czytelność kodu oraz łatwość poruszania się po nim.
- **Możliwość szybkiego rozpoczęcia pisania programów** - język C? będzie można wykorzystywać jako język skryptowy, dzięki czemu nie będzie konieczności uwzględniania sporej ilości kodu po to, aby program mógł się uruchomić.
- **Bezpieczeństwo** - działanie na wysokim poziomie, kontrola dostępu do danych oraz odporność interpretera na sytuacje nietypowe uczynią język C? bezpiecznym.

### Sposób uruchomienia programu oraz interakcja z interpreterem

Interpreter języka C? udostępnia CLI umożliwiające uruchamianie programów z terminala. Sposób uruchomienia programu przeprowadzany jest w sposób analogiczny do języka Python:

```bash
cnull [ścieżka-do-pliku-startowego]
```

Konwencjonalnie pliki zawierające kod języka C? mają rozszerzenie `.cnull`. Istnieje również możliwość szybszego uruchamiania programów C?, jeśli pracujemy z wieloplikowym projektem. Jeżeli w katalogu roboczym znajduje się plik `Program.cnull` (konwencjonalnie traktowany jako "punkt wejścia" programu), to do jego uruchomienia wystarczy wywołanie polecenia:

```bash
cnull
```

Jeżeli nie znajdujemy się w katalogu z projektem, możemy także użyć polecenia:

```bash
cnull [ścieżka-do-katalogu]
```

Spowoduje to uruchomienie pliku `Program.cnull` w podanym katalogu lub zasygnalizowanie błędu w przypadku niepowodzenia.

**Inne polecenia interpretera:**
- `cnull --help` - wyświetlenie instrukcji interpretera
- `cnull --version` - wyświetlenie używanej wersji interpretera

## Specyfikacja języka

### Elementy języka

#### Literały, identyfikatory i operatory

1. **Stałe**
	- **stałe liczbowe** - interpretowane w formacie dziesiętnym. Niedozwolone jest poprzedzanie stałych zerami.
		- całkowitoliczbowe - `0, -250, 12345`
		- zmiennoprzecinkowe - `0.24`, `-1.4567`, `1234.5678`, `0.290`. 
	- **stałe tekstowe** - ujmowane znakiem cudzysłowu - `"przykładowy tekst"`, `"Ala ma kota"`
	- **stałe znakowe** - ujmowane znakiem apostrofu. Mogą zawierać jedynie pojedynczy znak - `'a', 'A', 'h'`
	- **stałe boolowskie** - `true`, `false`
	- **stała wartości pustej** - `null`
2. **Identyfikatory** - mogą zawierać jedynie litery, znaki podkreślenia oraz cyfry (z czego cyfry nie może być znakiem rozpoczynającym identyfikator). Zasady tworzenia identyfikatorów są takie same dla zmiennych i funkcji. Maksymalna długość identyfikatora wynosi 256 znaków. `myVariable`, `MYVARIABLE`, `My_Variable`, `_myVariable1`, `____`. 
3. **Operatory**
	- **matematyczne** - `+`, `-`, `*`, `/`, `%`
	- **priorytetu** - `(`, `)`
	- **przypisania** - `=`
	- **porównania** - `<`, `<=`, `>`, `>=`, `==`, `!=`
	- **boolowskie** - `&&`, `||`, `!`, `?` (sprawdzenie, czy wartość jest pusta)
	- **wyłuskania** - `.`
4. **Oznaczenie granic bloku kodu** - `{`, `}`
5. **Komentarze** - `// przykładowy komentarz jednolinijkowy`

### Typy danych

Język C? jest typowany **statycznie i silnie**. Domyślnie, **każdy typ danych akceptuje wartość `null`.** Język C?, podobnie jak język C#, dzieli typy na 2 podstawowe grupy:
- **typy wartościowe** - w przypadku C? są to wszystkie typy prymitywne (łącznie ze `string`). Są one **niemutowalne**, **kopiowane przy przypisaniu** oraz **przekazywane przez wartość** jako argument.
- **typy referencyjne** - czyli klasy. Są one **mutowalne**, przy przypisaniu **kopiowana jest referencja**, a jako argument są one **przekazywane przez referencję**.

#### Typy wartościowe

1. **`bool`** - typ boolowski
2. **`int`** - typ całkowitoliczbowy (32-bitowa liczba całkowita ze znakiem)
3. **`float`** - liczba zmiennoprzecinkowa pojedynczej precyzji
4. **`string`** - ciąg znaków
5. **`char`** - pojedynczy znak

#### Wyjątki

Wyjątki w C? są de facto stringami, które można rzucać i obsługiwać tak, jak standardowe wyjątki. Zdefiniowany jest zestaw kilku standardowych wyjątków, które są rzucane np. w przypadku nieprawidłowego rzutowania lub odwołania do wartości `null`.

### Dane w programie

- Typowanie w języku C? jest **silne i statyczne**
- Argumenty do funkcji przekazywane są w zależności od ich typu:
	- typy prymitywne - **przez wartość**
	- typy złożone - **przez referencję**
- **Konwersje typów**:
	- następują niejawnie, ponieważ **operator przypisania jednocześnie jest w stanie wykonać rzutowanie w razie potrzeby**. Np. chcąc przypisać wartość typu `float` do typu `int`, zostanie ona automatycznie zrzutowana do typu docelowego, a jeśli to się nie powiedzie, to rzucony zostanie wyjątek.
- **Zakres zmiennej** jest ograniczony do bloku kodu, w której jest zdefiniowana, a także do bloków podrzędnych
- **Funkcje mogą być przeciążane** w obrębie pliku. Przeciążone metody muszą mieć co najmniej różny typ zwracany lub różne listy parametrów
- **Przykrywanie zmiennych** może występować podczas zdefiniowania zmiennej lokalnej w funkcji, kiedy już zdefiniowano zmienną o tej samej nazwie w pliku. Wówczas w danym kontekście brana pod uwagę jest ta zmienna, która jest na najniższym poziomie zagnieżdżenia. Nie można redefiniować pól i zmiennych lokalnych.
- W przypadku importowania elementów o tych samych nazwach, należy odnosić się do nich po pełnej nazwie (tj. razem z nazwą modułu).

### Obsługa błędów

Błędy będą obsługiwane przez osobny moduł projektu. Każde wystąpienie błędu w danym module interpretera (np. lekserze, parserze czy samym interpreterze) będzie sygnalizowane zgłoszeniem błędu, którego obsługa zostanie oddelegowana do modułu obsługi błędów. Na podstawie informacji przekazanych w błędzie podejmowane będą odpowiednie działania, jak np. wyświetlenie informacji na ekranie, zapis do logu itp.

Obsługiwane rodzaje błędów:
1. **Błędy dostępu do źródła** - zgłaszane w momencie nieprawidłowości odwołania do źródła kodu (np. próba odwołania do nieistniejącego pliku)
2. **Błędy kompilacji** - błędy związane ze statyczną analizą kodu (wszelkie błędy składniowe, semantyczne lub leksykalne)
3. **Błędy czasu wykonania** - sygnalizowane w czasie interpretacji programu (np. nieprawidłowe rzutowanie)

Komunikaty o błędach sformatowane będą w następujący sposób:
- dla błędów wykrytych przed wykonaniem:

```bash
C? error: [komunikat o błędzie]
	in [nazwa pliku] (line [nr linii], column [nr kolumny])
```

- dla błędów czasu wykonania (nieobsłużonych wyjątków) - wypisanie stosu wywołań:

```
C? unhandled exception ([komunikat z wyjątku]):
	at [nazwa funkcji, w której wystąpił wyjątek] (line [nr linii])
```

### Przykłady użycia języka

#### Przykłady elementarne - zmienne, importowanie bibliotek, operacje na danych prymitywnych i interakcja z terminalem

##### Sytuacje poprawne

**1. Obliczenie sumy liczb podanych przez użytkownika**

```csharp
import CNull.Console.WriteLine;
import CNull.Convert.ConvertToInt;

int first = ConvertToInt(ReadLine("Podaj pierwsza liczbe: "));
int second = ConvertToInt(ReadLine("Podaj druga liczbe: "));

int result = first + second;
WriteLine(result);
```

Wynik:
```
Podaj pierwsza liczbe: 30
Podaj druga liczbe: 20
50
```

**2. Złożona operacja matematyczna z niejawnymi konwersjami**

```csharp
import CNull.Console;

int a = 23;
short b = 14;

int c = (b + a) * (2 - b) / 2;
WriteLine(c);
```

Wynik:
```
-222
```

**3. Złożona operacja matematyczna z automatycznym rzutowaniem typu**

```csharp
import CNull.Console;

int a = 23;
short b = 14;

short c = (short)((b + a) * (2 - b) / 2);
WriteLine(c);
```

Wynik:
```
-222
```

**4. Deklaracje zmiennych wraz i bez inicjalizacji oraz demonstracja typów danych**

```csharp
int a;
a = 2000;
a = -2000;

short b = 0;
b = 300;

char c = 'A';
c = '';
string d = "Ala ma kota";
d = '';

bool e = true;
e = false;

float f = 2.1234355;
f = -2.424244;
f = 123123.123123;
f = 0.123123;
f = .12312313;

a = null;
b = null;
c = null;
d = null;
e = null;
f = null;
```
 
**5. Prezentacja różnych możliwych identyfikatorów**

```csharp
int someVariable;
int _someVariable;
int SOME_VARIABLE;
int _;
int ________;
int someVariable1;
int some_variable_1;
int _12345;
```

**6. Wyrażenia boolowskie**

```csharp
import CNull.Console.WriteLine;

bool a = true;
bool b = false;
bool c = null;
bool d = !(a || b) && a?;

WriteLine(a?);
WriteLine(c?);
WriteLine(c);
WriteLine(d);
```

Wynik:
```
true
false
null
false
```

##### Sytuacje niepoprawne

**1. Dyrektywy `import` niebędące na początku pliku**

```csharp
bool a = true;
import CNull.Console.WriteLine;
WriteLine(a);
```

Wynik:
```
C? error on line 2, column 1: Import statement must be placed at the top of the file.
```

**2. Użycie słowa kluczowego jako identyfikatora**

```csharp
int bool = 2;
```

Wynik:
```
C? error on line 1, column 5: Keywords cannot be used as identifiers.
```

**3. Odwołanie do wartości `null`**

```csharp
int a = 1;
int b;

int c = a + b;
```

Wynik:
```
C? unhandled exception (NullValueException - Tried to access null value in a non-nullable statement):
	at Program.cnull (line 4)
```

#### Przykłady złożone - funkcje, instrukcje sterujące i obsługa wyjątków

**1. Rekurencyjna funkcja silni z komentarzami i zasygnalizowaniem wynikiem `null` niepoprawnych danych wejściowych. Przykład z przepełnieniem stosu**

```csharp
import CNull.Console.WriteLine;

int Factorial(int n)
{
	if (!n?) // Check if the given value isn't null. Return null if yes.
	{
		return null;
	}

	if (n == 0 || n == 1)
	{
		return 1;
	}
	else
	{
		return n * Factorial(n - 1);
	}
}

WriteLine(Factorial(0));
WriteLine(Factorial(1));
WriteLine(Factorial(5));
WriteLine(Factorial(null));
WriteLine(Factorial(1000000000));
```

Wynik:
```
1
1
120
null
C? unhandled exception (Stack overflow):
	at Program.cnull (line 24)
	at Program.cnull (line 16)
```

**2. Definicja własnego wyjątku, rzucenie go w przykładowej funkcji oraz jego obsługa.**

```csharp
import CNull.Console.WriteLine;
import CNull.Convert.ConvertToString;

exception MyCustomException
{
	int InvalidValue;
}

void Foo(int a, int b)
{
	if (b > a)
	{
		throw new MyCustomException(b);
	}
}

try
{
	Foo(20, 10);
	Foo(10, 10);
	Foo(10, 20);
	Foo(10, 40);
	Foo(null, null);
}
catch (MyCustomException ex)
{
	WriteLine("Invalid value was: " + ConvertToString(ex.InvalidValue));
}

Foo(null, 20);
```

Wynik:
```
Invalid value was: 20
C? unhandled exception (NullValueException - Tried to access null value in a non-nullable statement):
	at Program.cs (line 28)
	at Program.cs (line 11)
```


**3. Przekazywanie przez wartość typów prymitywnych**

```csharp
import CNull.Console.WriteLine;

int a = 20;

void Process(int value)
{
	while (value <= 50)
	{
		value = value + 1;
	}

	WriteLine(value);
}

Process(a);
WriteLine(a);
```

Wynik:
```
50
20
```

**4. Operacje na słowniku**

```csharp
import CNull.Console.WriteLine;

dict<int, bool> d;

WriteLine(dict.Count());

d.Add(1, true);
d.Add(2, false);
d.Add(3, null);

WriteLine(d.Count());
WriteLine(d.Get(1));
WriteLine(d.Get(3));
WriteLine(d.Get(200));

d.Add(null, null);
```

Wynik:
```
0
3
true
null
null
C? unhandled exception (NullValueException - Cannot add a dictionary entry with null key):
	at Program.cnull (line 16)
```

**5. Wielokrotne, zagnieżdżone wywołania funkcji razem z przekazywaniem argumentu przez wartość**

```csharp
import CNull.Console.WriteLine;

int a = 20;

int A(int val)
{
	if (val < 10)
	{
		return 5;
	}

	return val + 40;
}

int B(int val)
{
	int counter = 0;
	while (counter < 5)
	{
		val = val + 1;

		if (val > 50)
		{
			return val;
		}

		counter = counter + 1;
	}

	return A(val);
}

int C(int val)
{
	if (val > 20)
	{
		return A(val);
	}

	return B(val);
}

WriteLine(C(100));
WriteLine(C(10));
WriteLine(C(a));
WriteLine(a);
```

Wynik:
```
140
55
65
20
```

**6. Definicja klasy i przekazywanie przez referencję typu złożonego.**

```csharp
import CNull.Console.WriteLine;
import CNull.Convert.ConvertToString;

class Person
{
	int Age = 20;
	string Name;

	string IntroduceMyself()
	{
		if(Age? || Name?)
		{
			return null;
		}

		return "Hi, my name is " + Name + " and I'm " + ConvertToString(Age) + " years old.";
	}
}

Person p = new Person;

void SetPersonInfo(Person person, int age, string name)
{
	person.Age = age;
	person.Name = name;
}

SetPersonInfo(p, 10, "Bruce");
WriteLine(p.IntroduceMyself());
```

Wynik:
```
Hi, my name is Bruce and I'm 10 years old.
```
### Opis gramatyki EBNF

#### Warstwa leksykalna

Opis gramatyki na poziomie leksykalnym znajduje się w pliku [lexical_grammar.ebnf](docs/lexical_grammar.ebnf).

#### Warstwa składniowa

Opis gramatyki na poziomie składni znajduje się w pliku [syntactic_grammar.ebnf](docs/syntactic_grammar.ebnf).

## Realizacja projektu

### Struktura projektu

Projekt C? będzie zrealizowany w języku C# w formie modularnej. Każdy moduł realizowany jest przez osobny projekt biblioteki klas C# albo aplikację konsolową jako warstwa front-end.

Główne moduły składające się na projekt:
- **`CNull.Source`** - biblioteka klas realizująca dostęp do źródła kodu oraz udostępnianie ich lekserowi w zunifikowanej formie nadającej się do przeprowadzenia analizy leksykalnej. Główne elementy składające się na tę bibliotekę:
	- Interfejs pobierania znaków ze źródła udostępniany lekserowi
	- Klasa realizująca dostęp do danych z plików i ich przetwarzanie z pomocą strumieni
	- Elementy pomocnicze dla warstwy dostępu do danych
	- Zdarzenia błędów charakterystyczne dla procesu dostępu do danych
- **`CNull.Lexer`** - biblioteka klas realizująca analizę leksykalną, tworzenie tokenów i udostępnianie ich parserowi w formie nadającej się do przeprowadzenia analizy składniowej. Główne elementy składające się na tę bibliotekę:
	- Analizator leksykalny (w tym jego interfejs udostępniany parserowi)
	- Generyczna implementacja tokenu
	- Enumeracje i mapy dla typów tokenów
	- Zdarzenia błędów charakterystyczne dla analizy leksykalnej
	- Elementy pomocnicze dla analizatora leksykalnego
- **`CNull.Parser`** - biblioteka klas realizująca analizę składniową, tworzenie drzewa rozbioru składniowego i udostępnienie go w formie nadającej się do przeprowadzenia analizy semantycznej. Główne elementy składające się na tę bibliotekę:
	- Analizator składniowy (w tym jego interfejs udostępniany analizatorowi semantycznemu)
	- Abstrakcja pozwalająca na reprezentację drzewa tokenów
	- Implementację proxy realizującego filtrowanie tokenów komentarzy
	- Zdarzenia błędów charakterystyczne dla analizy składniowej
	- Elementy pomocnicze dla analizatora składniowego
- **`CNull.Semantics`** - biblioteka klas realizująca analizę semantyczną otrzymanego drzewa rozbioru składniowego i umożliwienie wykonanie programu interpreterowi w formie sekwencji instrukcji. Główne elementy składające się na tę bibliotekę:
	- Analizator semantyczny (w tym jego interfejs udostępniany interpreterowi)
	- Abstrakcję pozwalającą na reprezentację instrukcji
	- Zdarzenia błędów charakterystyczne dla analizy semantycznej
	- Elementy pomocnicze dla analizatora semantycznego
- **`CNull.Interpreter`** - biblioteka klas stanowiąca implementację interpretera - w tym miejscu wykonywany jest program C?. Główne elementy składające się na tę bibliotekę:
	- Interpreter (w tym jego interfejs udostępniany modułom klienckim, w tym wypadku aplikacji konsolowej)
	- Zdarzenia błędów charakterystyczne dla interpretera
	- Elementy pomocnicze dla interpretera
- **`CNull`** - aplikacja konsolowa realizująca interakcję z użytkownikiem oraz obsługę jego poleceń. Posiada odwołanie do interpretera, któremu zleca wykonanie programu podanego przez użytkownika. Jej jedyną odpowiedzialnością jest obsługa CLI, wypisywanie błędów na ekran (zgłaszanych przez zdarzenia modułu obsługi błędów) oraz przekazywanie polecenia wykonania programu interpreterowi.

Dodatkowe moduły:
- **`CNull.ErrorHandler`** - moduł odpowiadający za obsługę błędów zgłaszanych przez poszczególne warstwy programu. Jego zadaniem jest subskrypcja zdarzeń za pośrednictwem agregatora zdarzeń, następnie ich obsługa (na którą mogą składać się jakiekolwiek dodatkowe czynności potrzebne przy ich obsłudze, np. zrzucanie pewnych informacji do logów) oraz przekazanie informacji o błędzie "front-endowi" w formie pojedynczego zdarzenia, z informacjami koniecznymi do realizacji wyświetlenia błędu.
- **`CNull.Common`** - moduł zawierający elementy pomocnicze i wspólne dla wszystkich składników programu.

Projekty testów:
- **`CNull.*.Tests`** - testy jednostkowe, gdzie `*` jest nazwą modułu głównego, np. `CNull.Lexer.Tests`
- **`CNull.IntegrationTests`** - testy integracyjne

Komunikacja między głównymi modułami następuje w taki sposób, że dany moduł ma powiązania jedynie z modułem znajdującym się bezpośrednio "pod nim" (tj. lekser może mieć zależność jedynie od źródła, parser od leksera itd.). Każdy z modułów głównych ma dostęp do modułu obsługi błędów. Wszystkie moduły mają dostęp do modułu elementów wspólnych.

Aby ułatwić testowanie oraz zarządzanie zależnościami, główne obiekty będą operować na interfejsach swoich zależności, a zarządzanie nimi będzie realizowane przez dependency injection.

### Testowanie

Jak w każdym złożonym projekcie, będzie można wyróżnić testy jednostkowe i integracyjne:
- **testy jednostkowe** będą ściśle odseparowywać wszelkie zależności od testowanych elementów, aby zapewnić jak najlepszą izolację przypadków testowych od pozostałych elementów i zapewnić atomowość takich testów. W szczególności, źródło danych będzie mockowane i przekazywane będą dane w postaci stringów. Do mockowania wykorzystany zostanie framework *Moq*.
- **testy integracyjne** będą sprawdzały poprawność współpracy kilku komponentów jednocześnie, od współpracy "w parach" (np. parsera z lekserem), jak i bardziej złożonej. 
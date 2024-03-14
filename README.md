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
- Struktura kodu napisanego w języku C? przypomina tę znaną z języka Python (tzn. w pliku źródłowym mogą być zarówno instrukcje, jak i deklaracje funkcji oraz klas).
- **Domyślna akceptacja wartości *null*** przez wszystkie definiowane typy, w tym wszystkie typy prymitywne.
- Wbudowana **kolekcja danych (słownik)**, będąca przykładem typu złożonego, posiadająca własny interfejs umożliwiający dodawanie, usuwanie, wyszukiwanie i sortowanie elementów. **Klucz w takim słowniku nie może mieć wartości *null***.
- **Sygnalizowanie błędów przy próbie operacji na wartości *null*** (tj. rzucenie wyjątku `NullValueException` w czasie wykonania programu).
- **Możliwość obsługi wyjątków** w blokach *try-catch*, znanych z innych języków programowania. Dodatkowo, możliwe jest utworzenie bloku *finally* - ma on tę samą funkcję, jak ten znany z języka C#.
- **Programista może tworzyć własne typy wyjątków.** Wyjątki są osobnym typem danych. Mogą one zawierać pola, ale nie mogą zawierać metod. Mogą być one używane jedynie zgodnie ze swoim przeznaczeniem (tj. jedynie rzucane w instrukcjach `throw` oraz obsługiwane w blokach `catch`)
- **Możliwość agregowania zmiennych i funkcji w klasy**, jednak bez zapewnienia mechanizmów znanych z typowych języków obiektowych (tj. dziedziczenia, hermetyzacji i polimorfizmu). Klasy w języku C? służą jedynie agregacji danych i funkcji oraz możliwości tworzenia prostych obiektów. Mechanizm ten jest niezbędny do tego, aby można było tworzyć czytelne w użyciu złożone typy danych (jak ww. wbudowany słownik).
- **Możliwość importowania klas, funkcji oraz wyjątków z innych plików**, na takiej samej zasadzie jak w przypadku języka Python.

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

%%- struktura programu (z jakich elementów może się składać program)
- omówienie poszczególnych elementów języka

    - literały (w tym stałe liczbowe, operatory)
    - typy danych
    - zmienne, wyrażenia
    - funckje
    - bloki try-catch-finally
    - klasy%%

#### Wykaz leksemów

1. **Słowa kluczowe**
	- typy danych/zwracane z funkcji lub metody
		- `bool`
		- `char`
		- `string`
		- `sbyte`
		- `byte`
		- `short`
		- `ushort`
		- `int`
		- `uint`
		- `long`
		- `ulong`
		- `void`
	- stałe
		- `null`
		- `true`
		- `false`
	- instrukcje sterujące
		- `if`
		- `else`
		- `while`
		- `return`
		- `throw`
		- `try`
		- `catch`
		- `finally`
	- deklaracje typów złożonych
		- `class`		
		- `exception`
	- pozostałe
		- `new`
		- `import`
2. **Operatory**
	- matematyczne - `+`, `-`, `*`, `/`
	- priorytetu - `(`, `)`
	- przypisania - `=`
	- porównania - `<`, `<=`, `>`, `>=`, `==`, `!=`
	- boolowskie - `&&`, `||`, `!`, `?`
	- odwołania do znaku w ciągu - `[`, `]`
	- wyłuskania - `.`
3. **Identyfikatory**
4. **Ograniczenia bloków kodu** - `{`, `}`
5. **Komentarz** - `// ...`
6. **Oddzielenie argumentów/parametrów funkcji** - `,`
7. **Koniec instrukcji** - `;`
8. **Stałe**
	- liczbowe
	- tekstowe
	- znakowe
	- określone ww. słowami kluczowymi

#### Literały, identyfikatory i operatory

1. **Stałe**
	- **stałe liczbowe** - interpretowane w formacie dziesiętnym. Niedozwolone jest poprzedzanie stałych zerami.
		- całkowitoliczbowe - `0, -250, 12345`
		- zmiennoprzecinkowe - `0.24`, `-1.4567`, `1234.5678`, `.290`. W przypadku stałych zmiennoprzecinkowych nieposiadających części całkowitej dozwolone jest pominięcie zera na początku.
	- **stałe tekstowe** - ujmowane znakiem cudzysłowu - `"przykładowy tekst"`, `"Ala ma kota"`
	- **stałe znakowe** - ujmowane znakiem apostrofu. Mogą zawierać jedynie pojedynczy znak - `'a', 'A', 'h'`
	- **stałe boolowskie** - `true`, `false`
	- **stała wartości pustej** - `null`
2. **Identyfikatory** - mogą zawierać jedynie litery, znaki podkreślenia oraz cyfry (z czego cyfry nie może być znakiem rozpoczynającym identyfikator). Zasady tworzenia identyfikatorów są takie same dla zmiennych, klas, funkcji, metod oraz wyjątków. Maksymalna długość identyfikatora wynosi 256 znaków. `myVariable`, `MYVARIABLE`, `My_Variable`, `_myVariable1`, `____`. 
3. **Operatory**
	- **matematyczne** - `+`, `-`, `*`, `/`
	- **priorytetu** - `(`, `)`
	- **przypisania** - `=`
	- **porównania** - `<`, `<=`, `>`, `>=`, `==`, `!=`
	- **boolowskie** - `&&`, `||`, `!`, `?` (sprawdzenie, czy wartość jest pusta)
	- **odwołania do znaku w ciągu** - `[]`
	- **wyłuskania** - `.`
1. **Oznaczenie granic bloku kodu** - `{`, `}`
2. **Komentarze** - `// przykładowy komentarz jednolinijkowy`

### Typy danych

Język C? jest typowany **statycznie i silnie**. Domyślnie, **każdy typ danych akceptuje wartość `null`.** Język C?, podobnie jak język C#, dzieli typy na 2 podstawowe grupy:
- **typy wartościowe** - w przypadku C? są to wszystkie typy prymitywne (łącznie ze `string`). Są one **niemutowalne**, **kopiowane przy przypisaniu** oraz **przekazywane przez wartość** jako argument.
- **typy referencyjne** - czyli klasy. Są one **mutowalne**, przy przypisaniu **kopiowana jest referencja**, a jako argument są one **przekazywane przez referencję**.

#### Typy wartościowe

1. **`bool`** - typ boolowski
2. **Typy całkowitoliczbowe**
	1. **`sbyte`** - 8-bitowa liczba całkowita ze znakiem
	2. **`byte`** - 8-bitowa liczba całkowita bez znaku
	3. **`short`** - 16-bitowa liczba całkowita ze znakiem
	4. **`ushort`** - 16-bitowa liczba całkowita bez znaku
	5. **`int`** - 32-bitowa liczba całkowita ze znakiem
	6. **`uint`** - 32-bitowa liczba całkowita bez znaku
	7. **`long`** - 64-bitowa liczba całkowita ze znakiem
	8. **`ulong`** - 64-bitowa liczba całkowita bez znaku
3. **`float`** - liczba zmiennoprzecinkowa podwójnej precyzji
4. **`string`** - ciąg znaków
5. **`char`** - pojedynczy znak

#### Klasy

Programista ma możliwość tworzenia własnych prostych klas (słowo kluczowe `class`), w skład których mogą wchodzić zmienne oraz metody. Każda klasa jest typem referencyjnym.

Język C? posiada **wbudowany słownik**, który w rzeczywistości jest klasą będącą częścią "biblioteki standardowej". Posiada zestaw metod umożliwiających operacje na nim.

#### Wyjątki

Dodatkowo, język C? wyróżnia **wyjątki jako osobny typ danych** (słowo kluczowe `exception`). **Wyjątki są typem wartościowym** - pomimo, że mogą być używane jedynie podczas obsługi wyjątków (co różni język C? od innych języków programowania, gdzie wyjątki to de facto zwykłe klasy), to podczas ich rzucenia ich wartość jest kopiowana od miejsca utworzenia do miejsca przechwycenia, a same w sobie są niemutowalne.

C? posiada zestaw wbudowanych wyjątków:
- `Exception`
- `NullValueException`
- `InvalidCastException`
- `DivideByZeroException`
- `IndexOutOfRangeException`

### Dane w programie

- opis typowania
- określenie zasad przekazywania argumentów
- określenie zasad konwersji (jawnych i niejawnych)
- określenie zasad zakresu zmiennych
- określenie zasad przeciążania funkcji
- określenie zasad przykrywania zmiennych

### Obsługa błędów

- błędy na poziomie leksera
- błędy na poziomie parsera
- błędy semantyczne
- błędy czasu wykonania
- jak będą wyglądały komunikaty o błędach

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

**3. Złożona operacja matematyczna z rzutowaniem typu**

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

ulong Factorial(uint n)
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
C? unhandled exception (StackOverflowException - Stack overflow):
	at Program.cnull (line 24)
	at Program.cnull (line 16)
```

**2. Definicja własnego wyjątku, rzucenie go w przykładowej funkcji oraz jego obsługa.

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

Dictionary<int, bool> dict;

WriteLine(dict.Count());

dict.Add(1, true);
dict.Add(2, false);
dict.Add(3, null);

WriteLine(dict.Count());
WriteLine(dict.Get(1));
WriteLine(dict.Get(3));
WriteLine(dict.Get(200));

dict.Add(null, null);
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

### Opis gramatyki EBNF

- opis w postaci top-down

## Realizacja projektu

### Struktura projektu

- z jakich elementów logicznych będzie się składał projekt (w tym z jakich projektów C#-owych, jakich bibliotek klas)
- rozpisanie komunikacji między poszczególnymi elementami

### Testowanie

- opis rodzajów testów
- jakie przypadki testowe będą testowane
- metodyka testów (użyte frameworki, jak umożliwimy testowalność komponentów)
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
	- odwołania do znaku w ciągu - `[]`
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
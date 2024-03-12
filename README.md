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
- `cnull --help` - wyświetlenie instrukcji inter
- `cnull --version` - wyświetlenie używanej wersji interpretera

## Specyfikacja języka

### Elementy języka

- struktura programu (z jakich elementów może się składać program)
- omówienie poszczególnych elementów języka

    - literały (w tym stałe liczbowe, operatory)
    - typy danych
    - zmienne, wyrażenia
    - funckje
    - bloki try-catch-finally
    - klasy

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

### Opis gramatyki EBNF

- opis w postaci top-down

### Przykłady użycia języka

## Realizacja projektu

### Struktura projektu

- z jakich elementów logicznych będzie się składał projekt (w tym z jakich projektów C#-owych, jakich bibliotek klas)
- rozpisanie komunikacji między poszczególnymi elementami

### Testowanie

- opis rodzajów testów
- jakie przypadki testowe będą testowane
- metodyka testów (użyte frameworki, jak umożliwimy testowalność komponentów)
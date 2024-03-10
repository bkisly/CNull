# C? - dokumentacja wstępna języka

## Informacje ogólne

- ogólne założenia co do języka, wytłumaczenie nazwy języka
- zakładane idee, konstrukcje, główne cechy
- interpretowany/kompilowany, zasada działania i uruchamiania programów

### Wymagania funkcjonalne/niefunkcjonalne

### Sposób uruchomienia programu

- z pliku
- opis CLI

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

## Realizacja projektu

### Struktura projektu

- z jakich elementów logicznych będzie się składał projekt (w tym z jakich projektów C#-owych, jakich bibliotek klas)
- rozpisanie komunikacji między poszczególnymi elementami

### Testowanie

- opis rodzajów testów
- jakie przypadki testowe będą testowane
- metodyka testów (użyte frameworki, jak umożliwimy testowalność komponentów)
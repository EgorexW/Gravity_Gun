# Kilka zdań o zastosowanych rozwiązaniach i problemach podczas implementacji.
## Input
Centralny skrypt, subskrybuje do Input Actions, parsuje inputy i wywołuje odpowiednie metody w innych komponentach.
## Pociski
Samo odbicie to jest prosta formuła: wyliczenie normalnej, użycie Vector2.Reflect

Z początku był problem, kiedy pocisk trafiał w róg. Rozwiązałem to dodając average normal ze wszystkich collision contactów. 

Żeby pociski nie trafiały w gracza, obok którego się pojawiają, przez pierwszy bardzo krótki czas kolizje są ignorowane.
## Cele
Spawn Point: Bierzemy losowy punkt w bounds zdefiniowanego collidera, sprawdzamy, czy jest valid. Szukamy do limitu prób.

TargetsManager nie powinien wiedzieć, czy cel ma Damageable, więc Target zbiera to w jednym miejscu.
## Gravity Bend
Physics2D.OverlapCircleAll, dla każdego w zasięgu obliczamy siłę przyciągania i dodajemy do velocity.

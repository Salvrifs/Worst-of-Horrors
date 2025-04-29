# speaker: ShrekFeya
# cooldown: 0.1
Эй, путник! Что привело тебя в мои топи? <#FF0000::SwampMagic/(3d)> 

+ [Что это за место?] -> swamp_info  
+ [Как выбраться?] -> escape_plan  
+ [Ты кто?] -> shrek_identity  

=== swamp_info ===
# speaker: ShrekFeya  
Здесь болото снов. Каждый камень здесь — твоё забытое воспоминание.  
(Вода хлюпает под ногами)  
-> END  

=== escape_plan ===
# speaker: ShrekFeya  
Варианты:  
- Собери 5 болотных огней (ищи у корней)  
- Или... останься со мной навеки.  

+ [А если я откажусь?] -> refuse_option  
+ [Хорошо, попробую] -> agree  

=== refuse_option ===
# speaker: ShrekFeya  
(Её глаза вспыхивают зеленым)  
Тогда ты станешь частью моего болота... как они.  
-> END  

=== agree ===
# speaker: ShrekFeya  
# method: ExitForGame  
Умный выбор. Ищи огни там, где тень целует воду.  
-> END  

=== shrek_identity ===
# speaker: ShrekFeya  
Я — дух этих топей. Когда-то была человеком... пока не съела <color=red>Кровавый Гриб</color>.  
+ [И это навсегда?] -> forever_question  

=== forever_question ===
(Она смеется, и эхо разносится по болоту)  
Вечность — понятие растяжимое.  
-> END  
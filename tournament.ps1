#CSharpEngine.exe "maps/map41.txt" 2000 100 "log.txt" "Bot.exe" "java -jar example_bots/DualBot.jar" | java -jar tools/ShowGame.jar

$root_dir = "C:\Projects\pw\"
$my_bot = "bot.exe"
$enemy_bot = "bot_exe\bot3.exe"

cd $root_dir
[IO.Directory]::SetCurrentDirectory($root_dir)

#$bots = Get-ChildItem ("example_bots") | where {$_.extension -eq ".jar"}

#foreach($bot in $bots) {
    $botname = "bot3"#$bot.name 
    "Bot: " + $botname
    
    $player_1_counter=0
    $player_2_counter=0
    $draw_counter=0
    $maps_played=0
    
    foreach($i in (1..100)) {   
        $output = .\CSharpEngine.exe maps\map$i.txt 3000 200 log.txt $my_bot $enemy_bot 2>&1
        
        if($output -match "Player 1 Wins!") {
            $player_1_counter = $player_1_counter + 1
            $i.toString() + " - win"
        }
        elseif($output -match "Player 2 Wins!") {
            $player_2_counter = $player_2_counter + 1
            $i.toString() + " - lose"
        }
        else {
            $draw_counter = $draw_counter + 1
            $i.toString() + " - draw"
        }
        
        $maps_played = $maps_played + 1
    }
    
    "won against " + $botname + ": " + $player_1_counter + "/" + $maps_played
    "lost against " + $botname + ": " + $player_2_counter+"/"+$maps_played
    "draws: " + $draw_counter+"/"+$maps_played
#}
#CSharpEngine.exe "maps/map41.txt" 2000 100 "log.txt" "Bot.exe" "java -jar example_bots/DualBot.jar" | java -jar tools/ShowGame.jar

$root_dir = "C:\Projects\pw\"
$my_bot = "bot.exe"
#$enemy_bot = "bot_exe\bot3.exe"

#cd $root_dir
#[IO.Directory]::SetCurrentDirectory($root_dir)

#$bots = Get-ChildItem ("bot_exe") | where {$_.extension -eq ".exe"}
$bots = @()
#$bots += "bot_exe\bot1.exe"
#$bots += "bot_exe\bot3.exe"
$bots += "bot_exe\bot4.exe"
#$bots += '"java -jar example_bots/DualBot.jar"'

foreach($bot in $bots) {
    $botname = $bot        #.name 
    "Bot: " + $botname
    
    $player_1_counter=0
    $player_2_counter=0
    $draw_counter=0
    $maps_played=0
    
    foreach($i in (1..100)) {   
        $output = .\CSharpEngine.exe "maps\map$i.txt" 3000 200 log.txt $my_bot $botname 2>&1
        
        if($output -match "Player 1 Wins!") {
            $player_1_counter = $player_1_counter + 1
            #$i.toString() + " - win"
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
    
    "    won  : " + $player_1_counter + "/" + $maps_played
    "    lost : " + $player_2_counter+"/"+$maps_played
    if ($draw_counter>0) {"draws: " + $draw_counter+"/"+$maps_played}
}
#CSharpEngine.exe "maps/map41.txt" 2000 100 "log.txt" "Bot.exe" "java -jar example_bots/DualBot.jar" | java -jar tools/ShowGame.jar

$root_dir = "C:\Projects\pw\"
$my_bot = "bot.exe"
#$enemy_bot = "bot_exe\bot3.exe"

#cd $root_dir
#[IO.Directory]::SetCurrentDirectory($root_dir)

#$bots = Get-ChildItem ("bot_exe") | where {$_.extension -eq ".exe"}
$bots = @()
#$bots += "bot_exe\bot1.exe"
#$bots += "bot_exe\bot2.exe"
#$bots += "bot_exe\bot3.exe"
#$bots += "bot_exe\bot4.exe"
#$bots += "bot_exe\bot5.exe"
#$bots += "bot_exe\bot6.exe"
$bots += "bot_exe\bot7.exe"
#$bots += '"java -jar example_bots/RageBot.jar"'
#$bots += '"java -jar example_bots/DualBot.jar"'
#$bots += '"java -jar example_bots/BullyBot.jar"'
#$bots += '"java -jar example_bots/ProspectorBot.jar"'
#$bots += '"java -jar example_bots/RandomBot.jar"'
$bots += "bot_exe\HazardBot_12.exe"
$bots += "bot_exe\HazardBot_18.exe"
#$bots += "bot_exe\HazardBot_9.exe"
#$bots += "bot_exe\HazardBot_10.exe"
#$bots += "bot_exe\HazardBot_11.exe"
#$bots += "bot_exe\Torquemada 3.exe"
#$bots += "bot_exe\mubot.exe"
#$bots += "bot_exe\BlueSky9.exe"

foreach($bot in $bots) {
    $botname = $bot        #.name 
    "Bot: " + $botname
    
    $player_1_counter=0
    $player_2_counter=0
    $draw_counter=0
    $maps_played=0
    
    foreach($i in (1..30)) {   
	if ($botname -match "java") {
		$output = java.exe -jar tools\PlayGame.jar maps\map_r528_$i.txt 3000 200 log.txt $my_bot "$botname" 2>&1
	}
	else {
        	$output = .\CSharpEngine.exe "maps\map_r528_$i.txt" 3000 200 "log.txt" $my_bot $botname 2>&1
	}
        
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
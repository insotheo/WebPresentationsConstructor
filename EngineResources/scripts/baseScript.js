
//_EDITOR::FUNCTIONS::TYPES::EVENT
function say(message){
    console.log("WPC_Editor::User: " + message);
    alert(message);
}

//_EDITOR::FUNCTIONS::TYPES::EVENT
function quit(){
    window.close();
}

//_EDITOR::FUNCTIONS::TYPES::EVENT
function askThenQuit(){
    if(confirm("Вы уверены, что хотите закрыть страницу?") == true){
        window.close();
    }
}
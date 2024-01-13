
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

//_EDITOR::FUNCTIONS::TYPES::EVENT
function goPage(page_name){
    if (page_name != null) {
        let str_page_name = page_name.toString();
        let doesPageExist = false;
        if(!str_page_name.endsWith(".html")){
            str_page_name += ".html";  
        } 
        let http = new XMLHttpRequest();
        http.open("HEAD", str_page_name, true);
        http.send();
        if(http.status != 404){
            doesPageExist = true;
        }
        if (doesPageExist) {
            window.location.href = str_page_name
        }
        else{
            alert("Ошибка открытия страницы!");
            console.log("ERROR: page to open(${str_page_name}) doesn't exist![WPC]");
        }
    }
    else{
        console.log("ERROR: null ref exeption![WPC]");
    }
}

//_EDITOR::FUNCTIONS::TYPES::EVENT
function reloadPage(){
    location.reload();
}

//_EDITOR::FUNCTIONS::TYPES::EVENT
function replaceText(element_name, new_text){
    if(element_name != null){
        let el = document.getElementById(element_name);
        if(el != null){
            el.textContent = new_text;
            console.log("DONE![WPC]");
        }
        else{
            console.log("ERROR: null ref exeption![WPC]");
        }
    }
}

function replaceText(element_name, old_text_part, new_text_part){
    if(element_name != null){
        let el = document.getElementById(element_name);
        if(el != null){
            el.textContent = el.textContent.replace(old_text_part, new_text_part);
            console.log("DONE![WPC]");
        }
        else{
            console.log("ERROR: null ref exeption![WPC]");
        }
    }
}

//_EDITOR::FUNCTIONS::TYPES::EVENT
function removeElement(element_name){
    if(element_name != null){
        let elID = document.getElementById(element_name);
        if(elID != null){
            elID.remove();
        }
        else{
            console.log("ERROR: null ref exeption![WPC]");
        }
    }
    else{
        console.log("ERROR: null ref exeption![WPC]");
    }
}
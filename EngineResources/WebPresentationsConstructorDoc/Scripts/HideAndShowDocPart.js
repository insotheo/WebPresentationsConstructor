function toggleContent(divId){
    let container = document.getElementById(divId);
    let isNowClosed = false;
    if(container.style.display === "none"){
        container.style.display = "block";
    }
    else{
        container.style.display = "none";
        isNowClosed = true;
    }
    if(container.className === "contentSection" && !isNowClosed){
        let allElementsArray = document.getElementsByClassName("contentSection");
        for(let i = 0; i < allElementsArray.length; i++){
            if(allElementsArray[i].id != divId){
                allElementsArray[i].style.display = "none";
            }
        }
    }
}
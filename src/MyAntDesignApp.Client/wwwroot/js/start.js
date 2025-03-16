window.init = function() {
    var style = ["style1", "style2", "style3", "style4"];
    var tam = ["tam1", "tam1", "tam1", "tam2", "tam3"];
    var opacity = ["opacity1", "opacity1", "opacity1", "opacity2", "opacity2", "opacity3"];

    function getRandomArbitrary(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    var estrela = "";
    var qtdeEstrelas = 1000;
    var noite = document.querySelector(".constelacao");
    var widthWindow = window.innerWidth;
    var heightWindow = window.innerHeight;

    for (var i = 0; i < qtdeEstrelas; i++) {
        estrela += "<span class='estrela " + style[getRandomArbitrary(0, 4)] + " " + opacity[getRandomArbitrary(0, 6)] + " "
        + tam[getRandomArbitrary(0, 5)] + "' style='animation-delay: ." + getRandomArbitrary(0, 9) + "s; left: "
        + getRandomArbitrary(0, widthWindow) + "px; top: " + getRandomArbitrary(0, heightWindow) + "px;'></span>";
    }

    noite.innerHTML = estrela;

    var numeroAleatorio = 2000;

    setTimeout(function () {
        carregarMeteoro();
    }, numeroAleatorio);

    function carregarMeteoro() {
        var meteorosAoMesmoTempo = getRandomArbitrary(3, 20);
        for (var i = 0; i < meteorosAoMesmoTempo; i++) {
            var meteoro = "<div class='meteoro " + style[getRandomArbitrary(0, 4)] + "'></div>";
            document.getElementsByClassName('chuvaMeteoro')[0].innerHTML += meteoro;
        }

        setTimeout(function () {
            document.getElementsByClassName('chuvaMeteoro')[0].innerHTML = "";
        }, 1000);

        numeroAleatorio = getRandomArbitrary(2000, 5000);
        setTimeout(carregarMeteoro, numeroAleatorio);
    }
};

window.highlightReply = function (selectedId) {
    // 取得已經被選中的元素
    let selectedElement = document.getElementById(selectedId);

    // 檢查如果該元素已經有 selected-reply 類別，則移除，反之則加上
    if (selectedElement) {
        if (selectedElement.classList.contains("selected-reply")) {
            // 已經選中，再次點擊取消選中
            selectedElement.classList.remove("selected-reply");
        } else {
            // 移除所有元素的 selected-reply 類別
            document.querySelectorAll(".comment-action").forEach(el => {
                el.classList.remove("selected-reply");
            });
            // 加上選中的元素的 selected-reply 類別
            selectedElement.classList.add("selected-reply");
        }
    }
};

window.updateCalendarHeader = function (year, month) {
    let elements = document.querySelectorAll(".ant-picker-calendar-header .ant-select-selection-item");

    if (elements.length >= 2) {
        elements[0].innerText = year + "年";  // 第一個是年份
        elements[1].innerText = month + "月"; // 第二個是月份
    }
};
window.init = function() {
    // estrelas (星星部分不需要修改)
    var style = ["style1", "style2", "style3", "style4"];
    var tam = ["tam2"];
    var opacity = ["opacity1", "opacity1", "opacity1", "opacity2", "opacity2", "opacity3"];
  
    function getRandomArbitrary(min, max) {
      return Math.floor(Math.random() * (max - min)) + min;
    }
  
    var estrela = "";
    var qtdeEstrelas = 100;
    var noite = document.querySelector(".constelacao");
    var widthWindow = window.innerWidth;
    var heightWindow = window.innerHeight;
  
    for (var i = 0; i < qtdeEstrelas; i++) {
      estrela += "<span class='estrela-sun " + style[getRandomArbitrary(0, 4)] + " " + opacity[getRandomArbitrary(0, 6)] + " "
        + tam[getRandomArbitrary(0, 5)] + "' style='animation-delay: ." + getRandomArbitrary(0, 9) + "s; left: "
        + getRandomArbitrary(0, widthWindow) + "px; top: " + getRandomArbitrary(0, heightWindow) + "px;'></span>";
    }
  
    noite.innerHTML = estrela;
  
    // meteoros
    var numeroAleatorio = 2000; // 缩短初始间隔
  
    setTimeout(function () {
      carregarMeteoro();
    }, numeroAleatorio);
  
    function carregarMeteoro() {
      // 每次调用生成 3~5 个流星
      var meteorosAoMesmoTempo = getRandomArbitrary(3, 6);
      for (var i = 0; i < meteorosAoMesmoTempo; i++) {
        var meteoro = "<div class='meteoro " + style[getRandomArbitrary(0, 4)] + "'></div>";
        document.getElementsByClassName('chuvaMeteoro')[0].innerHTML += meteoro;
      }
  
      // 清理之前的流星
      setTimeout(function () {
        document.getElementsByClassName('chuvaMeteoro')[0].innerHTML = "";
      }, 1000);
  
      // 下次生成的间隔
      numeroAleatorio = getRandomArbitrary(2000, 5000); // 缩短随机间隔
      setTimeout(carregarMeteoro, numeroAleatorio);
    }
  };
  
  
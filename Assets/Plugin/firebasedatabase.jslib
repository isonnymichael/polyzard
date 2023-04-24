mergeInto(LibraryManager.library, {

    getData: function(_address) {
        var address = UTF8ToString(_address);
         _wall = address;
        try {

             firebase.database().ref(`idle-game/${address}`).once('value').then(function(snapshot) {
               if(snapshot.val()){
                    console.log(snapshot.val());
                    
                    window._unityInstance.SendMessage("Menu", "OnGetDataFB", JSON.stringify(snapshot.val()));
                }else{
                    window._unityInstance.SendMessage("Menu", "OnGetDataFB", JSON.stringify(register(address)));
                }
            });

        } catch (error) {
           return 'error';
        }
    },

    updgradeDamage: function(_address, _skillID, _damage) {
        var address = UTF8ToString(_address);
        var skillID = _skillID;
        var damage = _damage;
        console.log("upgrade damage execute")
        try {
           
            firebase.database().ref(`idle-game/${address}/skills/${skillID}/damageLevel`).set(damage).then(function(unused) {
                console.log(_damage);
            });

        } catch (error) {
            console.log(error);
        }
    },

    updgradeSpeed: function(_address, _skillID, _speed) {
        var address = UTF8ToString(_address);
        var skillID = _skillID;
        var speed = _speed;
        console.log("upgrade speed execute")
        try {

            firebase.database().ref(`idle-game/${address}/skills/${skillID}/speedLevel`).set(speed).then(function(unused) {
                console.log(_speed);
            });

        } catch (error) {
           console.log(error);
        }
    },

    unlockSkill: function(_address, _skillID) {
        var address = UTF8ToString(_address);
        var skillID = _skillID;

        try {

             firebase.database().ref(`idle-game/${address}/skills/${skillID}/unlocked`).set(true).then(function(unused) {
                console.log(skillID)
            });

        } catch (error) {
          console.log("error")
        }
    },

    changeActiveSkill: function(_address, _skillID) {
        var address = UTF8ToString(_address);
        var skillID = _skillID;

        try {

             firebase.database().ref(`idle-game/${address}/activatedSkillID`).set(_skillID).then(function(unused) {
                console.log(_skillID);
            });

        } catch (error) {
           console.log("error")
        }
    },

    nextLevel: function(_address, _level) {
        var address = UTF8ToString(_address);
        var level = _level;

        try {

            firebase.database().ref(`idle-game/${address}/level`).set(level).then(function(unused) {
                console.log(level);
            });

        } catch (error) {
           console.log("error")
        }
    },

    getTempMoney: function(_address, _tempMoney) {
        var address = UTF8ToString(_address);
        var tempMoney = _tempMoney;
        console.log("getTempMoney");
        try {

            firebase.database().ref(`idle-game/${address}/tempMoney`).set(tempMoney).then(function(unused) {
                console.log(tempMoney);
            });

        } catch (error) {
           console.log("error")
        }
    },

    getLeaderboard: function(_limit) {
        var limit = _limit;

        try {

            firebase.database().ref('idle-game').orderByChild('-level').limitToFirst(limit).once('value', function(snapshot) {
                snapshot.forEach(function(childSnapshot) {
                    return JSON.stringify(snapshot.val());
                });
            });

        } catch (error) {
           return 'error';
        }
    }

});
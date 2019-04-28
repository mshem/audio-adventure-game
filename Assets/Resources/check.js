const fs = require("fs");

const jsonString = fs.readFileSync(__dirname + "/zorblok.json");
const json = JSON.parse(jsonString);
const dialogs = fs.readdirSync(__dirname + "/Audio/zorblok-dialog/").map(name => "zorblok-dialog/" + name);
const sound = fs.readdirSync(__dirname + "/Audio");

const fileNames = [...dialogs, ...sound]
    // .map(f => {
    //     return f;
    // })
    .map(file => file.split(".")[0])
    .filter(f => !!f)
    .reduce((acc, cur) => {
        if(!acc[cur]) {
            acc[cur] = 0;
        }
        return acc; 
}, {});

const errors = Object
    .keys(json)
    .map(key => json[key].soundQueue)
    .reduce((acc, cur) =>  {
        acc.push(...cur);
        return acc;
    }, [])
    .map(f => f.fileLoc)
    .reduce((acc, cur) => {
        if(fileNames[cur] !== undefined) {
            fileNames[cur] += 1;
        } else {
            acc.push(cur);
        }
        return acc;
    }, []);

errors.forEach(error => {
   console.error(`${error} is missing from directory!`);
});

Object.keys(fileNames).forEach(name => {
    if(fileNames[name] > 0) {
        console.log(`${name} is used ${fileNames[name]} times`);
    } else {
        console.warn(`${name} is never used`);
    }
})
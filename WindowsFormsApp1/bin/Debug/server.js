const express = require('express');
const mysql = require('mysql');
const app = express();
const port = 3000;

// 设置数据库连接
const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: 'Hans_12345',
    database: 'emily'
});

// 连接到数据库
db.connect((err) => {
    if (err) throw err;
    console.log('Connected to database');
});

// 获取当前级别
app.get('/api/level', (req, res) => {
    const query = 'SELECT currentLevel FROM rewardSetting WHERE id = 1';
    db.query(query, (err, result) => {
        if (err) throw err;
        res.json({ currentLevel: result[0].currentLevel });
    });
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
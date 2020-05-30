<style>
a,p,h1,h2,h3,h4,h5,h6 {
    text-align: center;
}

.search {
    /*
    box-shadow: 8px 8px 4px #888888;
    border: 2px solid black;
    */
}

.entity-table {
    display: flex;
    justify-content: space-around;
    flex-direction: row;
    flex-wrap: wrap;
}

.searchTitle {
    text-align: center;
}

.searchForm {
    margin-left: 10px;
    margin-bottom: 20px;
}

</style>
</head>
<body>

<article>
    
<section>
    <h1>Poco</h1>
    <hr/>
    <h2>Search</h2>
    <hr/>
</section>
    
<section class="search">
    <div class="entity-table">
    <?php 
        foreach ($entities as $entity)
            require(realpath(dirname(__FILE__) . './templates/entity.php'));
    ?>
    </div>
</section>

<section>
    <p>Cygnut @ 2016</p>
</section>

</article>
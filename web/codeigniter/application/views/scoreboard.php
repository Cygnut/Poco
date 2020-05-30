<!DOCTYPE html>
<html>
<head>
<style>

a,p,h1,h2,h3,h4,h5,h6 {
    text-align: center;
}

.entity-table {
    display: flex;
    justify-content: space-around;
    flex-direction: row;
    flex-wrap: wrap;
}

h3.scoreTitle {
    text-align: center;
}

/* The following two are to spread out the buttons evenly horizontally. */
div.links {
    margin-top: 25px;
    display: table;
    width: 100%;
    table-layout: fixed;    /* For cells of equal size */
}

div.links span {
    display: table-cell;
    text-align: center;
}

</style>
    
    <title>Poco | Scoreboard</title>
    
    <meta charset="UTF-8">
    <meta name="description" content="Poco | Scoreboard">
    <meta name="keywords" content="Film, TV, Music, Food">
    <meta name="author" content="Cygnut">
    
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <link rel="shortcut icon" href="img/content/PocoIcon-64x64-Transparent.png" />
    
</head>
<body>
    
    <article>
    
    <section>
        <h1>Poco</h1>
        <hr/>
        <h2>Scoreboard</h2>
        <hr/>
    </section>
    
    <section class="scoreboard" >
        <h3 class="scoreTitle">Rankings (<?php echo $filter ?>)</h3>
        <br/>
        
        <div class="entity-table">
            <?php 
                foreach ($entities as $entity)
                    require(realpath(dirname(__FILE__) . './templates/entity.php'));
            ?>
        </div>
        
        <div class="links">
            <span><?php echo $links; ?></span>
        </div>
    </section>
    
    <section>
        <p>Cygnut @ 2016</p>
    </section>
    
    </article>
    
</body>
</html>
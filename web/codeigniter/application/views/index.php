<style>

section {
    margin: auto;
}

a,p,h1,h2,h3,h4,h5,h6 {
    text-align: center;
}

.entity-section {
    background-color: teal;
    width: 900px;
    height: 900px;
    
    border-radius: 450px;
    
    /* Centre the internal div. */
    display: flex;
    justify-content: center;
    align-items: center;
}

.entity-container {
    /* Space out content evenly. */
    width: 100%;
    display: flex;
    justify-content: space-around;
}

.links-container {
    text-align: center;
}

.links-container ul {
    /* No bullet points. */
    list-style-type: none;
    /* No margin where the bullet points were. */
    margin: 0;
    padding: 0;
}

.links-container ul li {
    display: inline;
}

form {
    text-align: center;
}

/* Initially hide entity cards. */
entity {
    display: none;
}

</style>

<script src="<?php echo base_url('resources/vendor/jquery-3.1.1.min.js'); ?>"></script>
<script src="<?php echo base_url('resources/vendor/jquery-ui.min-1.12.1.js'); ?>"></script>
<script>
    $(function() {
        
        $(".entity")
            .hide()
            .show("drop", { direction: "up" }, 1000);
        
        $(".entity")
            .click(function() {
                id = $(this).attr('data-id');
                // TODO: Handle this!
            });
        
    });
    
    function searchSubmit() {
        window.location.href = 'search/' + $('#fragment').val();
        return false;
    }
</script>

</head>
<body>

<article>

    <section>
        <h1>Poco</h1>
        <hr/>
        <h2>It's a popularity contest!</h2>
        <hr/>
    </section>
    
    <br/>
    
    <!-- Voting section -->
    <section class="entity-section">
        
        <div class="entity-container">
        <?php
            foreach ($votableEntities as $entity)
                require(realpath(dirname(__FILE__) . './templates/entity.php'));
        ?>
        </div>
        
    </section>
    
    <br/>
    
    <hr/>
        
    <section>
        <h3>Scoreboard</h3>
        
        <div class="links-container">
            
            <ul>
            <li><a href="/scoreboard/All">All</a></li>
            <?php
                foreach ($categories as $category):
            ?>
            <li>
                <a href="/scoreboard/<?php echo $category['name']; ?>"><?php echo $category["name"]; ?></a>
            </li>
            <?php endforeach; ?>
            </ul>
        
        <div>
        
    </section>
    
    <hr>
    
    <section>
        <h3>Search</h3>
        
        <form class="searchForm" onsubmit="return searchSubmit();" method="GET">
            <input type="text" name="fragment" id="fragment"></input>
            <input type="submit" value="Search"></input>
        </form>
        
        <br/>
    </section>
    
    <hr/>
    
    <section>
        <p>Cygnut @ 2016</p>
    </section>

</article>
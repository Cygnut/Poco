DELIMITER $$
DROP PROCEDURE IF EXISTS `i_updateEntity`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `i_updateEntity`(
    in _name varchar(255), 
    in _category_id int,
    in _sub_category varchar(255),
    in _blurb varchar(511),
    in _image_url varchar(511),
    in _native_popularity int
    )
BEGIN
    
    declare _cur_id int;
    declare _cur_sub_category varchar(255);
    declare _cur_blurb varchar(511);
    declare _cur_image_url varchar(511);
    declare _cur_native_popularity int;
    
    declare _inserted int; 
    declare _updated int; 
        
    set _inserted = 0;
    set _updated = 0;
    
    # Search for the existing category.names existence
    select 
        id,
        sub_category,
        blurb,
        image_url,
        native_popularity
    into
        _cur_id,
        _cur_sub_category,
        _cur_blurb,
        _cur_image_url,
        _cur_native_popularity
    from entity 
    where 
        name = _name and
        category_id = _category_id
    limit 1;  
    
    if _cur_id is not null
    then
        # Then such a category.name exists - check if we need to update any related fields.
        if 
            _cur_sub_category <> _sub_category or
            _cur_blurb <> _blurb or
            _cur_image_url <> _image_url or
            _cur_native_popularity <> _native_popularity
        then
            update entity set 
                sub_category = _sub_category, 
                blurb = _blurb,
                image_url = _image_url,
                native_popularity = _native_popularity,
                when_updated = now()
            where 
                id = _cur_id;
            
            set _updated = _updated + 1;
        end if;
    else
        # Then such an category.name entity does not exist. So insert it.
        insert into entity 
            (name, category_id, sub_category, blurb, image_url, native_popularity) 
        values 
            (_name, _category_id, _sub_category, _blurb, _image_url, _native_popularity);
    
        select last_insert_id() into _cur_id;
        
        # Also insert into entity_score.
        insert into entity_score (entity_id) values (_cur_id);
        
        set _inserted = _inserted + 1;
        
    end if;
    
    select
        _cur_id as id,
        _inserted as inserted,
        _updated as updated;        
    
END$$
DELIMITER ;

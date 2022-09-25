<xsl:stylesheet
        version="1.1"
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:ext="http://exslt.org/common"
        exclude-result-prefixes="ext"
        xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    >

    <xsl:template name="tokenize">
        <xsl:param name="text"/>
            <xsl:choose>
                <xsl:when test="translate($text, ',', '') = $text">
                    <xsl:element name="AvailablePiece">
                        <xsl:attribute name="id">
                            <xsl:value-of select="$text"/>
                        </xsl:attribute>
                    </xsl:element>                    
                </xsl:when>
                <xsl:otherwise>
                    <xsl:element name="AvailablePiece">
                        <xsl:attribute name="id">
                            <xsl:value-of select="substring-before($text, ',')"/>
                        </xsl:attribute>
                    </xsl:element>
                    <xsl:call-template name="tokenize">
                        <xsl:with-param name="text" select="substring-after($text, ',')"/>
                    </xsl:call-template>
                </xsl:otherwise>
            </xsl:choose>
    </xsl:template>

    <xsl:template name="generate-available-pieces">
        <xsl:param name="weapon-pieces"/>

        <xsl:variable name="available-piece-tags">
            <xsl:call-template name="tokenize">
                <xsl:with-param name="text">
                    <xsl:value-of select="$weapon-pieces"/>
                </xsl:with-param>
            </xsl:call-template>
        </xsl:variable>
        
        <xsl:copy-of select="."/>
        <xsl:for-each select="msxsl:node-set($available-piece-tags)">
            <xsl:copy-of select="."/>
        </xsl:for-each>
    </xsl:template>

    <xsl:variable name="two-handed-mace-pieces" select="concat(
            'cs_maul_tip_1,',
            'cs_morningstar_shaft,',
            'cs_morningstar_head,',
            'cs_wooden_morningstar_head,',
            'cs_wooden_morningstar_head_2,',
            'cs_wooden_morningstar_head_3,',
            'cs_s_morningstar_shaft,',
            'cs_ls_morningstar_shaft,',
            'cs_club_handle,',
            'cs_no_head,',
			'mace_head_7,',
			'mace_handle_15'
        )"/>

    <xsl:variable name="javelin-pieces" select="'cs_bamboo_spear_handle'"/>

    <xsl:variable name="pike-pieces" select="concat(
					'cs_barbarian_pike_shaft,', 
					'spear_handle_3'
				)"/>

    <xsl:variable name="two-handed-axe-pieces" select="concat(
					'cs_pick_head,',
					'axe_craft_26_head'
				)"/>
    
    <xsl:variable name="two-handed-polearm-pieces" select="concat(
                    'cs_halberd_head,',
                    'cs_halberd_handle,',
                    'cs_harpoon_shaft,',
                    'cs_blunt_spear_head'
                )"/>

    <xsl:variable name="throwing-hammer-pieces" select="concat(
                    'axe_craft_11_handle,',
                    'axe_craft_12_handle,',
                    'axe_craft_28_handle,',
                    'mace_head_6,',
                    'cs_maul_tip_2,',
                    'cs_blunt_spear_head,',
                    'cs_throwing_club_handle,',
                    'mp_vlandian_throwing_axe_handle,',
                    'mp_vlandian_throwing_axe_handle_extra,',
                    'mp_sturgia_throwing_axe_handle,',
                    'mp_aserai_throwing_axe_handle'
                )"/>

    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedMace']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$two-handed-mace-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='Javelin']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="concat($javelin-pieces, ',', $two-handed-polearm-pieces)"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='OneHandedPolearm_JavelinAlternative']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$javelin-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedPolearm_Pike']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$pike-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedPolearm_Bracing']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="concat($pike-pieces, ',', $two-handed-polearm-pieces)"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedPolearm_Pike']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="concat($pike-pieces, ',', $two-handed-polearm-pieces)"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='OneHandedAxe']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="concat($two-handed-axe-pieces, ',', $throwing-hammer-pieces)"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedAxe']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$two-handed-axe-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='ThrowingAxe']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$throwing-hammer-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedPolearm_Couchable']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$two-handed-polearm-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='OneHandedPolearm']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$two-handed-polearm-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="//WeaponDescription[@id='TwoHandedPolearm']//AvailablePiece[not(following-sibling::AvailablePiece)]">
        <xsl:call-template name="generate-available-pieces">
            <xsl:with-param name="weapon-pieces">
                <xsl:value-of select="$two-handed-polearm-pieces"/>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>
    
</xsl:stylesheet>
